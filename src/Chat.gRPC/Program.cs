using Chat.gRPC.Interceptors;
using Chat.gRPC.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Chat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddGrpc(options => options.Interceptors.Add<ExceptionInterceptor>());
            builder.Services.AddSingleton<IChatRoomService, ChatRoomService>();

            //Set up Serilog
            using var log = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            builder.Services.AddSingleton<Serilog.ILogger>(log);
            log.Information("Done setting up serilog!");

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.MapGrpcService<ChatServer>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");


            app.Run();
        }
    }
}