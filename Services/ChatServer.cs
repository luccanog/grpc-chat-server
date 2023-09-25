using Grpc.Core;

namespace Chat.gRPC.Services
{
    public class ChatServer : ChatService.ChatServiceBase
    {
        private readonly ILogger<ChatServer> _logger;

        public ChatServer(ILogger<ChatServer> logger)
        {
            _logger = logger;
        }

        public override Task HandleCommunication(IAsyncStreamReader<ClientMessage> requestStream, IServerStreamWriter<ServerMessage> responseStream, ServerCallContext context)
        {
            return base.HandleCommunication(requestStream, responseStream, context);
        }
    }
}
