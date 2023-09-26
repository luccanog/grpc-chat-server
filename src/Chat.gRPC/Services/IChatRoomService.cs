using Chat.gRPC.Models;
using Grpc.Core;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Chat.gRPC.Services
{
    public interface IChatRoomService
    {
        public Task AddClientToChatRoomAsync(string chatRoomId, User user);

        public Task StreamClientJoinedRoomServerMessageAsync(string chatRoomId, string userName);

        public Task StreamServerMessageAsync(string chatRoomId, string senderName, string message);

        public ReadOnlyCollection<string> ListChatRooms();
    }
}
