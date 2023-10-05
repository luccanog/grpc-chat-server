using Chat.gRPC.Protos;
using Chat.SingalR.Hubs;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Chat.SingalR.Controllers
{
    public class ChatHub : Hub<IChat>
    {
        private Dictionary<string, ChatService.ChatServiceClient> _grpcClients = new();
        public override async Task OnConnectedAsync()
        {
            if (!_grpcClients.ContainsKey(Context.ConnectionId))
            {
                using var channel = GrpcChannel.ForAddress("https://localhost:7256");
                var client = new ChatService.ChatServiceClient(channel);
                _grpcClients.Add(Context.ConnectionId, client);
            }

            await Clients.All.SendMessageAsync($"{Context.ConnectionId} connected");
        }

        public async Task LoginAsync(string username, string chatRoomId)
        {
            var client = _grpcClients[Context.ConnectionId];

            var stream = client.HandleCommunication();

            await stream.RequestStream.WriteAsync(new ClientMessage { Login = new ClientMessageLogin { ChatRoomId = "123", UserName = "CDE" } });
        }
    }
}