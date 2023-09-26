using Grpc.Core.Testing;
using Grpc.Core;
using Chat.gRPC.Interceptors;
using Serilog;
using FluentAssertions;

namespace Chat.gRPC.Tests.Interceptors
{
    public class ExceptionHelperTests
    {
        private readonly ServerCallContext _serverCallContext;
        private readonly ILogger _logger;

        public ExceptionHelperTests()
        {
            _serverCallContext = TestServerCallContext.Create(String.Empty, String.Empty, DateTime.Now,
            new Metadata(), new CancellationTokenSource().Token, String.Empty, null,
            null, null, null, null);

            _logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.TestCorrelator().CreateLogger();
        }

        [Fact]
        public void ExceptionHelper_TimeOutException_ShouldBeHandled()
        {
            //Arrange
            var exception = new TimeoutException();
            var correlationId = Guid.NewGuid();

            //Act
            var rpcException = exception.Handle(_serverCallContext, _logger, correlationId);

            //Assert
            rpcException.Status.StatusCode.Should().Be(StatusCode.Internal);
        }

        [Fact]
        public void ExceptionHelper_RPCException_ShouldBeHandled()
        {
            //Arrange
            var exception = new RpcException(Status.DefaultCancelled);
            var correlationId = Guid.NewGuid();

            //Act
            var rpcException = exception.Handle(_serverCallContext, _logger, correlationId);

            //Assert
            rpcException.Status.StatusCode.Should().Be(StatusCode.Cancelled);
            rpcException.Trailers.Should().Contain(t => t.Value.Equals(correlationId.ToString()));
        }


        [Fact]
        public void ExceptionHelper_AnyKindOfException_ShouldBeHandled()
        {
            //Arrange
            var exception = new ArgumentNullException();
            var correlationId = Guid.NewGuid();

            //Act
            var rpcException = exception.Handle(_serverCallContext, _logger, correlationId);

            //Assert
            rpcException.Status.StatusCode.Should().Be(StatusCode.Internal);
            rpcException.Trailers.Should().Contain(t => t.Value.Equals(correlationId.ToString()));
        }

    }
}
