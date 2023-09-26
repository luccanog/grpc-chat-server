using AutoFixture;
using Chat.gRPC.Models;
using Chat.gRPC.Services;
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
            var user = _fixture.Create<User>();

            //Act
            await _chatRoomService.AddClientToChatRoomAsync(chatRoomId, user);

            //Assert
            

            await Task.CompletedTask;
        }
    }
}
