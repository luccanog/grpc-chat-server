﻿using Chat.gRPC.Models;
using Grpc.Core;

namespace Chat.gRPC.Services
{
    public interface IChatRoomService
    {
        public Task<ClientMessage> ReadMessageWithTimeoutAsync(IAsyncStreamReader<ClientMessage> requestStream, TimeSpan timeout);

        public Task AddClientToChatRoomAsync(string chatRoomId, User user);

        public Task StreamClientJoinedRoomServerMessageAsync(string chatRoomId, string userName);

        public Task StreamServerMessageAsync(string chatRoomId, string senderName, string message);
    }
}
