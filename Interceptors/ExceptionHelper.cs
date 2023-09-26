using Grpc.Core;
using Serilog;
using System;

namespace Chat.gRPC.Interceptors
{
    public static class ExceptionHelper
    {
        public static RpcException Handle(this Exception exception, ServerCallContext context, ILogger logger, Guid correlationId) =>
            exception switch
            {
                TimeoutException => HandleTimeoutException((TimeoutException)exception, context, logger, correlationId),
                RpcException => HandleRpcException((RpcException)exception, context, logger, correlationId),
                _ => HandleDefault(exception, context, logger, correlationId)
            };

        private static RpcException HandleTimeoutException(TimeoutException exception, ServerCallContext context, ILogger logger, Guid correlationId)
        {
            logger.Error(correlationId.ToString(), exception, context.Method);

            var status = new Status(StatusCode.Internal, exception.Message);

            return new RpcException(status, BuildResponseMetadata(correlationId));
        }

        private static RpcException HandleRpcException(RpcException exception, ServerCallContext context, ILogger logger, Guid correlationId)
        {
            logger.Error(correlationId.ToString(), exception, context.Method);
            return new RpcException(new Status(exception.StatusCode, exception.Message), BuildResponseMetadata(correlationId));
        }

        private static RpcException HandleDefault(Exception exception, ServerCallContext context, ILogger logger, Guid correlationId)
        {
            logger.Error(correlationId.ToString(), exception, context.Method);
            return new RpcException(new Status(StatusCode.Internal, exception.Message), BuildResponseMetadata(correlationId));
        }

        /// <summary>
        ///  Adding the correlation to Response Trailers
        /// </summary>
        /// <param name="correlationId"></param>
        /// <returns></returns>
        private static Metadata BuildResponseMetadata(Guid correlationId)
        {
            var trailers = new Metadata();
            trailers.Add("CorrelationId", correlationId.ToString());
            return trailers;
        }
    }
}
