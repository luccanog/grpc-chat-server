using Chat.gRPC.Protos;
using Grpc.Core;
using Serilog;
using System.Threading.Tasks;

namespace Chat.gRPC.Services
{
    public class ChatServer : ChatService.ChatServiceBase
    {
        private readonly IChatRoomService _chatRoomService;
        private readonly ILogger _logger;

        public ChatServer(IChatRoomService chatRoomService, ILogger logger)
        {
            _chatRoomService = chatRoomService;
            _logger = logger;
        }

        public override async Task HandleCommunication(IAsyncStreamReader<ClientMessage> requestStream, IServerStreamWriter<ServerMessage> responseStream, ServerCallContext context)
        {
            _logger.Information("Connection stablished");

            string userName = string.Empty;
            string chatRoomId = string.Empty;

            while (await requestStream.MoveNext(context.CancellationToken))
            {
                var clientMessage = requestStream.Current;

                switch (clientMessage.ContentCase)
                {
                    case ClientMessage.ContentOneofCase.Login:
                        userName = clientMessage.Login.UserName;
                        chatRoomId = clientMessage.Login.ChatRoomId;
                        _logger.Information("Login message received from {userName}", userName);

                        if (!await IsInformationValid(responseStream, userName, chatRoomId, "Invalid username or chat room"))
                        {
                            return;
                        }

                        await HandleLogin(responseStream, userName, chatRoomId);
                        break;

                    case ClientMessage.ContentOneofCase.Chat:
                        _logger.Information("Chat message received from {userName}", userName);

                        if (!await IsInformationValid(responseStream, userName, chatRoomId, "User not logged in"))
                        {
                            return;
                        }
                        await _chatRoomService.StreamServerMessageAsync(chatRoomId, userName, clientMessage.Chat.Text);
                        break;
                }
            }
        }

        private async Task HandleLogin(IServerStreamWriter<ServerMessage> responseStream, string userName, string chatRoomId)
        {
            await _chatRoomService.AddClientToChatRoomAsync(chatRoomId, new Models.User(userName, responseStream));

            var successMessage = new ServerMessage { LoginSuccess = new ServerMessageLoginSuccess() };
            await responseStream.WriteAsync(successMessage);

            await _chatRoomService.StreamClientJoinedRoomServerMessageAsync(chatRoomId, userName);
        }

        private async Task<bool> IsInformationValid(IServerStreamWriter<ServerMessage> responseStream, string userName, string chatRoomId, string reason)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(chatRoomId))
            {
                var failureMessage = new ServerMessage
                {
                    LoginFailure = new ServerMessageLoginFailure { Reason = reason }
                };
                await responseStream.WriteAsync(failureMessage);
                return false;
            }

            return true;
        }
    }

}

