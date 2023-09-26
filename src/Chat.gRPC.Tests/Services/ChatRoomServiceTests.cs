using AutoFixture;
using Chat.gRPC.Models;
using Chat.gRPC.Services;
using Grpc.Core;
using Serilog;


namespace Chat.gRPC.Tests.Services
{
    public class ChatRoomServiceTests
    {
        private readonly IChatRoomService _chatRoomService;
        private readonly Fixture _fixture;
        public ChatRoomServiceTests()
        {
            var logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.TestCorrelator().CreateLogger();
            _chatRoomService = new ChatRoomService(logger);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task AddClientToChatRoomAsync_WithInexistentChatRoom_ShouldCreateChatRoom()
        {
            //Arrange
            var chatRoomId = Guid.NewGuid().ToString();
            var user = new User("John Doe", new Mock<IServerStreamWriter<ServerMessage>>().Object);

            //Act
            await _chatRoomService.AddClientToChatRoomAsync(chatRoomId, user);

            //Assert
            var rooms = _chatRoomService.ListChatRooms();
            rooms.Should().HaveCount(1);
            rooms.First().Should().Be(chatRoomId);
        }
    }
}
