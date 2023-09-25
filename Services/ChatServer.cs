using Grpc.Core;

namespace Chat.gRPC.Services
{
    public class ChatServer : ChatService.ChatServiceBase
    {
        private readonly ILogger<ChatServer> _logger;
        private readonly IChatRoomService _chatRoomService;

        public ChatServer(ILogger<ChatServer> logger, IChatRoomService chatRoomService)
        {
            _logger = logger;
            _chatRoomService = chatRoomService;
        }

        public override Task HandleCommunication(IAsyncStreamReader<ClientMessage> requestStream, IServerStreamWriter<ServerMessage> responseStream, ServerCallContext context)
        {
            return base.HandleCommunication(requestStream, responseStream, context);
        }

    }
}
