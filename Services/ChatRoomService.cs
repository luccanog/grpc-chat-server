using Chat.gRPC.Models;
using Grpc.Core;
using System.Collections.Concurrent;

namespace Chat.gRPC.Services
{
    public class ChatRoomService : IChatRoomService
    {
        private readonly ILogger<ChatRoomService> _logger;
        private readonly ConcurrentDictionary<string, List<User>> _chatRooms;

        public ChatRoomService(ILogger<ChatRoomService> logger)
        {
            _logger = logger;
            _chatRooms = new ConcurrentDictionary<string, List<User>>();
        }

        public async Task<ClientMessage> ReadMessageWithTimeoutAsync(IAsyncStreamReader<ClientMessage> requestStream, TimeSpan timeout)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(timeout);

            try
            {
                bool isMovingNext = await requestStream.MoveNext(cancellationTokenSource.Token);

                if (!isMovingNext)
                {
                    throw new Exception("Connection dropped");
                }

                return requestStream.Current;
            }
            catch (TimeoutException)
            {
                _logger.LogError($"{nameof(ReadMessageWithTimeoutAsync)} - Timeout");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(ReadMessageWithTimeoutAsync)} - Error: {ex.Message}");
                throw;
            }
        }

        public async Task AddClientToChatRoomAsync(string chatRoomId, User user)
        {
            if (!_chatRooms.ContainsKey(chatRoomId))
            {
                _chatRooms[chatRoomId] = new List<User> { user };
            }
            else
            {
                var userAlreadyExist = _chatRooms[chatRoomId].Any(c => c.Name == user.Name);
                if (userAlreadyExist)
                {
                    throw new InvalidOperationException("User with the same name already exists in the chat room");
                }

                _chatRooms[chatRoomId].Add(user);
            }
            await Task.CompletedTask;
        }

        public async Task StreamClientJoinedRoomServerMessageAsync(string chatRoomId, string userName)
        {
            if (_chatRooms.ContainsKey(chatRoomId))
            {
                var serverMessage = new ServerMessage { UserJoined = new ServerMessageUserJoined { UserName = userName } };

                var tasks = new List<Task>();

                foreach (var user in _chatRooms[chatRoomId])
                {
                    if (user is not null)
                    {
                        tasks.Add(user.StreamWriter.WriteAsync(serverMessage));
                    }
                }

                await Task.WhenAll(tasks);
            }
        }

        public async Task StreamServerMessageAsync(string chatRoomId, string senderName, string message)
        {
            if (_chatRooms.ContainsKey(chatRoomId))
            {
                var tasks = new List<Task>();
                var serverMessage = new ServerMessage { Chat = new ServerMessageChat { UserName = senderName, Text = message } };

                foreach (var user in _chatRooms[chatRoomId])
                {
                    if (user is not null && user.Name != senderName)
                    {
                        tasks.Add(user.StreamWriter.WriteAsync(serverMessage));
                    }
                }
                await Task.WhenAll(tasks);
            }
        }
    }
}
