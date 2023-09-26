using Chat.gRPC.Models;
using Grpc.Core;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.gRPC.Services
{
    public class ChatRoomService : IChatRoomService
    {
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<string, List<User>> _chatRooms;

        public ChatRoomService(ILogger logger)
        {
            _logger = logger.ForContext<ChatRoomService>();
            _chatRooms = new ConcurrentDictionary<string, List<User>>();
        }

        public async Task<ClientMessage> ReadMessageAsync(IAsyncStreamReader<ClientMessage> requestStream)
        {
            try
            {
                bool isMovingNext = await requestStream.MoveNext();

                if (!isMovingNext)
                {
                    throw new Exception("Connection dropped");
                }

                return requestStream.Current;
            }
            catch (Exception)
            {
                _logger.Error($"{nameof(ReadMessageAsync)}");
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
