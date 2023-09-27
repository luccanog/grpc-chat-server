using Chat.gRPC.Models;
using Chat.gRPC.Protos;
using Chat.gRPC.Services;
using Grpc.Core;
using Grpc.Core.Testing;

namespace Chat.gRPC.Tests.Services
{
    public class ChatServerTests
    {

        private readonly Mock<IAsyncStreamReader<ClientMessage>> _requestStreamMock;
        private readonly Mock<IServerStreamWriter<ServerMessage>> _responseStreamMock;
        private readonly Mock<IChatRoomService> _chatRoomServiceMock;
        private readonly ChatServer _chatServer;

        public ChatServerTests()
        {
            _requestStreamMock = new Mock<IAsyncStreamReader<ClientMessage>>();
            _responseStreamMock = new Mock<IServerStreamWriter<ServerMessage>>();

            _chatRoomServiceMock = new Mock<IChatRoomService>();
            _chatServer = new ChatServer(_chatRoomServiceMock.Object);
        }

        [Fact]
        public async Task HandleCommunication_LoginWithValidData_ShouldSucceed()
        {
            //Arrange
            var chatRoomId = Guid.NewGuid().ToString();
            ServerCallContext context = SetupGrpcAuthenticationMocks();

            _requestStreamMock.SetupSequence(r => r.MoveNext(It.IsAny<CancellationToken>())).ReturnsAsync(true).ReturnsAsync(false);
            _requestStreamMock.Setup(r => r.Current).Returns(new ClientMessage() { Login = new ClientMessageLogin() { ChatRoomId = chatRoomId, UserName = Constants.DefaultName } });

            //Act
            await _chatServer.HandleCommunication(_requestStreamMock.Object, _responseStreamMock.Object, context);

            //Assert
            _chatRoomServiceMock.Verify(c => c.AddClientToChatRoomAsync(chatRoomId, It.IsAny<User>()), Times.Once);
            _chatRoomServiceMock.Verify(c => c.StreamClientJoinedRoomServerMessageAsync(chatRoomId, Constants.DefaultName), Times.Once);
        }

        [Fact]
        public async Task HandleCommunication_LoginWithEmptyUserName_ShouldFail()
        {
            //Arrange
            var chatRoomId = Guid.NewGuid().ToString();
            ServerCallContext context = SetupGrpcAuthenticationMocks();

            _requestStreamMock.SetupSequence(r => r.MoveNext(It.IsAny<CancellationToken>()))
                              .ReturnsAsync(true)
                              .ReturnsAsync(false);

            _requestStreamMock.Setup(r => r.Current)
                              .Returns(new ClientMessage() { Login = new ClientMessageLogin() { ChatRoomId = chatRoomId, UserName = string.Empty } });

            //Act
            await _chatServer.HandleCommunication(_requestStreamMock.Object, _responseStreamMock.Object, context);

            //Assert
            _chatRoomServiceMock.Verify(c => c.AddClientToChatRoomAsync(chatRoomId, It.IsAny<User>()), Times.Never);
            _chatRoomServiceMock.Verify(c => c.StreamClientJoinedRoomServerMessageAsync(chatRoomId, It.IsAny<string>()), Times.Never);

        }

        [Fact]
        public async Task HandleCommunication_SendMessageAsLoggedUser_ShouldSucceed()
        {
            //Arrange
            var chatRoomId = Guid.NewGuid().ToString();
            var messageContent = "dummy message";
            ServerCallContext context = SetupGrpcAuthenticationMocks();

            _requestStreamMock.SetupSequence(r => r.MoveNext(It.IsAny<CancellationToken>()))
                              .ReturnsAsync(true)
                              .ReturnsAsync(true)
                              .ReturnsAsync(false);

            _requestStreamMock.SetupSequence(r => r.Current)
                              .Returns(new ClientMessage() { Login = new ClientMessageLogin() { ChatRoomId = chatRoomId, UserName = Constants.DefaultName } })
                              .Returns(new ClientMessage() { Chat = new ClientMessageChat() { Text = messageContent } });

            //Act
            await _chatServer.HandleCommunication(_requestStreamMock.Object, _responseStreamMock.Object, context);

            //Assert
            _chatRoomServiceMock.Verify(c => c.StreamServerMessageAsync(chatRoomId, Constants.DefaultName, messageContent), Times.Once);
        }

        [Fact]
        public async Task HandleCommunication_SendMessageWithoutLoggingFirst_ShouldFail()
        {
            //Arrange
            var chatRoomId = Guid.NewGuid().ToString();
            var messageContent = "dummy message";
            ServerCallContext context = SetupGrpcAuthenticationMocks();

            _requestStreamMock.SetupSequence(r => r.MoveNext(It.IsAny<CancellationToken>()))
                              .ReturnsAsync(true)
                              .ReturnsAsync(false);

            _requestStreamMock.SetupSequence(r => r.Current)
                              .Returns(new ClientMessage() { Chat = new ClientMessageChat() { Text = messageContent } });

            //Act
            await _chatServer.HandleCommunication(_requestStreamMock.Object, _responseStreamMock.Object, context);

            //Assert
            _chatRoomServiceMock.Verify(c => c.StreamServerMessageAsync(chatRoomId, Constants.DefaultName, messageContent), Times.Never);
        }

        private ServerCallContext SetupGrpcAuthenticationMocks()
        {
            var serverCallContext = TestServerCallContext.Create(String.Empty, String.Empty, DateTime.Now,
                new Metadata(), new CancellationTokenSource().Token, String.Empty, null,
                null, null, null, null);

            return serverCallContext;
        }
    }
}
