using Chat.gRPC.Interceptors;
using Grpc.Core;
using Serilog;

namespace Chat.gRPC.Tests.Interceptors
{
    public class ExceptionInterceptorTests
    {
        private readonly ILogger _logger;
        private readonly Mock<ServerCallContext> _context;
        private readonly ExceptionInterceptor _interceptor;

        public ExceptionInterceptorTests()
        {
            _logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.TestCorrelator().CreateLogger();
            _context = new Mock<ServerCallContext>();
            _interceptor = new ExceptionInterceptor(_logger);
        }

        [Fact]
        public async Task ExceptionInterceptor_ShouldInterceptUnaryServerMethod()
        {
            //Arrange
            var serverMethod = new Mock<UnaryServerMethod<string, string>>();
            serverMethod.Setup(s => s.Invoke(It.IsAny<string>(), _context.Object)).Throws(new TimeoutException());

            //Act
            var exception = await Record.ExceptionAsync(async () =>
            {
                await _interceptor.UnaryServerHandler(It.IsAny<string>(), _context.Object, serverMethod.Object);
            });

            //Assert
            exception.Should().BeAssignableTo<RpcException>();
        }

        [Fact]
        public async Task ExceptionInterceptor_ShouldInterceptClientStreamingServerMethod()
        {
            //Arrange
            var serverMethod = new Mock<ClientStreamingServerMethod<string, string>>();
            serverMethod.Setup(s => s.Invoke(It.IsAny<IAsyncStreamReader<string>>(), _context.Object)).Throws(new TimeoutException());

            //Act
            var exception = await Record.ExceptionAsync(async () =>
            {
                await _interceptor.ClientStreamingServerHandler(It.IsAny<IAsyncStreamReader<string>>(), _context.Object, serverMethod.Object);
            });

            //Assert
            exception.Should().BeAssignableTo<RpcException>();
        }


        [Fact]
        public async Task ExceptionInterceptor_ShouldInterceptServerStreamingServerMethod()
        {
            //Arrange
            var serverMethod = new Mock<ServerStreamingServerMethod<string, string>>();
            serverMethod.Setup(s => s.Invoke(It.IsAny<string>(), It.IsAny<IServerStreamWriter<string>>(), _context.Object)).Throws(new TimeoutException());

            //Act
            var exception = await Record.ExceptionAsync(async () =>
            {
                await _interceptor.ServerStreamingServerHandler(It.IsAny<string>(), It.IsAny<IServerStreamWriter<string>>(), _context.Object, serverMethod.Object);
            });

            //Assert
            exception.Should().BeAssignableTo<RpcException>();
        }

        [Fact]
        public async Task ExceptionInterceptor_ShouldInterceptDuplexStreamingServerMethod()
        {
            //Arrange
            var serverMethod = new Mock<DuplexStreamingServerMethod<string, string>>();
            serverMethod.Setup(s => s.Invoke(It.IsAny<IAsyncStreamReader<string>>(), It.IsAny<IServerStreamWriter<string>>(), _context.Object)).Throws(new TimeoutException());

            //Act
            var exception = await Record.ExceptionAsync(async () =>
            {
                await _interceptor.DuplexStreamingServerHandler(It.IsAny<IAsyncStreamReader<string>>(), It.IsAny<IServerStreamWriter<string>>(), _context.Object, serverMethod.Object);
            });

            //Assert
            exception.Should().BeAssignableTo<RpcException>();
        }
    }
}
