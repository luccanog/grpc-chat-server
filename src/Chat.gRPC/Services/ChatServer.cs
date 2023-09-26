using Grpc.Core;
using System.Threading.Tasks;

namespace Chat.gRPC.Services
{
    public class ChatServer : ChatService.ChatServiceBase
    {
        private readonly IChatRoomService _chatRoomService;

        public ChatServer(IChatRoomService chatRoomService)
        {
            _chatRoomService = chatRoomService;
        }

        public override async Task HandleCommunication(IAsyncStreamReader<ClientMessage> requestStream, IServerStreamWriter<ServerMessage> responseStream, ServerCallContext context)
        {
            string userName = string.Empty;
            string chatRoomId = string.Empty;

            while (true)
            {
                var clientMessage = await _chatRoomService.ReadMessageAsync(requestStream);

                switch (clientMessage.ContentCase)
                {
                    case ClientMessage.ContentOneofCase.Login:
                        userName = clientMessage.Login.UserName;
                        chatRoomId = clientMessage.Login.ChatRoomId;
                        if (!await IsInformationValid(responseStream, userName, chatRoomId, "Invalid username or chat room"))
                        {
                            return;
                        }
                        await HandleLogin(responseStream, userName, chatRoomId);
                        break;

                    case ClientMessage.ContentOneofCase.Chat:
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

