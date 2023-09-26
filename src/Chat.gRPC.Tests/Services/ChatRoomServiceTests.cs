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
        
        public ChatRoomServiceTests()
        {
            var logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.TestCorrelator().CreateLogger();
            _chatRoomService = new ChatRoomService(logger);
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

        [Fact]
        public async Task AddClientToChatRoomAsync_WithAlreadyExistentChatRoom_ShouldCreateChatRoom()
        {
            //Arrange
            var chatRoomId = Guid.NewGuid().ToString();
            var user = new User("John Doe", new Mock<IServerStreamWriter<ServerMessage>>().Object);
            var user2 = new User("New User", new Mock<IServerStreamWriter<ServerMessage>>().Object);
            await _chatRoomService.AddClientToChatRoomAsync(chatRoomId, user);

            //Act
            await _chatRoomService.AddClientToChatRoomAsync(chatRoomId, user2);

            //Assert
            var rooms = _chatRoomService.ListChatRooms();
            rooms.Should().HaveCount(1);
            rooms.First().Should().Be(chatRoomId);
        }

        [Fact]
        public async Task AddClientToChatRoomAsync_Twice_ShouldThrowException()
        {
            //Arrange
            var chatRoomId = Guid.NewGuid().ToString();
            var user = new User("John Doe", new Mock<IServerStreamWriter<ServerMessage>>().Object);
            await _chatRoomService.AddClientToChatRoomAsync(chatRoomId, user);

            //Act
            var exception = await Record.ExceptionAsync(async () => await _chatRoomService.AddClientToChatRoomAsync(chatRoomId, user));

            //Assert
            exception.Should().BeAssignableTo<InvalidOperationException>();
        }
    }
}
