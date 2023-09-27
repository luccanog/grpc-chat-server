using Chat.gRPC.Models;
using Chat.gRPC.Protos;
using Chat.gRPC.Services;
using Grpc.Core;



namespace Chat.gRPC.Tests.Services
{
    public class ChatRoomServiceTests
    {
        private readonly IChatRoomService _chatRoomService;
        private readonly Mock<IServerStreamWriter<ServerMessage>> _defaultServerStreamWriterMock;

        private const string DefaultName = "John Doe";
        private readonly User _defaultUser;

        public ChatRoomServiceTests()
        {
            _chatRoomService = new ChatRoomService();
            _defaultServerStreamWriterMock = new Mock<IServerStreamWriter<ServerMessage>>();
            _defaultUser = new User(DefaultName, _defaultServerStreamWriterMock.Object);
        }

        [Fact]
        public async Task AddClientToChatRoomAsync_WithInexistentChatRoom_ShouldCreateChatRoom()
        {
            //Arrange
            var chatRoomId = Guid.NewGuid().ToString();

            //Act
            await _chatRoomService.AddClientToChatRoomAsync(chatRoomId, _defaultUser);

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
            await _chatRoomService.AddClientToChatRoomAsync(chatRoomId, _defaultUser);

            var secondUser = new User($"{DefaultName}123", _defaultServerStreamWriterMock.Object);

            //Act
            await _chatRoomService.AddClientToChatRoomAsync(chatRoomId, secondUser);

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
            await _chatRoomService.AddClientToChatRoomAsync(chatRoomId, _defaultUser);

            //Act
            var exception = await Record.ExceptionAsync(async () => await _chatRoomService.AddClientToChatRoomAsync(chatRoomId, _defaultUser));

            //Assert
            exception.Should().BeAssignableTo<InvalidOperationException>();
        }


        [Fact]
        public async Task StreamClientJoinedRoomServerMessageAsync_ShouldSucceed()
        {
            //Arrange
            var chatRoomId = Guid.NewGuid().ToString();
            var expectedMessage = new ServerMessage { UserJoined = new ServerMessageUserJoined { UserName = _defaultUser.Name } };
            await _chatRoomService.AddClientToChatRoomAsync(chatRoomId, _defaultUser);

            //Act
            await _chatRoomService.StreamClientJoinedRoomServerMessageAsync(chatRoomId, _defaultUser.Name);

            //Assert
            _defaultServerStreamWriterMock.Verify(s => s.WriteAsync(expectedMessage), Times.Once);
        }

        [Fact]
        public async Task StreamClientJoinedRoomServerMessageAsync_WhenChatRoomDoesNotExist_ShouldFail()
        {
            //Act
            var act = async () => await _chatRoomService.StreamClientJoinedRoomServerMessageAsync("inexistent-chat-room", _defaultUser.Name);
            var exception = await Record.ExceptionAsync(act);

            //Assert
            exception.Should().BeAssignableTo<InvalidOperationException>();
        }

        [Fact]
        public async Task StreamServerMessageAsync_WhenChatRoomDoesNotExist_ShouldFail()
        {
            //Act
            var act = async () => await _chatRoomService.StreamServerMessageAsync("inexistent-chat-room", _defaultUser.Name, "dummy message");
            var exception = await Record.ExceptionAsync(act);

            //Assert
            exception.Should().BeAssignableTo<InvalidOperationException>();
        }

        [Fact]
        public async Task StreamServerMessageAsync_UserShouldNotReceiveYourOwnMessage()
        {
            //Arrange
            var chatRoomId = Guid.NewGuid().ToString();
            var messageText = "dummy message";

            var expectedMessage = new ServerMessage { Chat = new ServerMessageChat { UserName = _defaultUser.Name, Text = messageText } };
            var recipientUserServerStreamWriter = new Mock<IServerStreamWriter<ServerMessage>>();
            var recipientUser = new User("John", recipientUserServerStreamWriter.Object);

            await _chatRoomService.AddClientToChatRoomAsync(chatRoomId, _defaultUser);
            await _chatRoomService.AddClientToChatRoomAsync(chatRoomId, recipientUser);

            //Act
            await _chatRoomService.StreamServerMessageAsync(chatRoomId, _defaultUser.Name, messageText);

            //Assert
            _defaultServerStreamWriterMock.Verify(s => s.WriteAsync(It.IsAny<ServerMessage>()), Times.Never);
            recipientUserServerStreamWriter.Verify(s => s.WriteAsync(expectedMessage), Times.Once);

        }
    }
}
