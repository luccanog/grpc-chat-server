using Chat.SingalR.Controllers;
using System.Diagnostics.CodeAnalysis;

namespace Chat.SingalR
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSignalR();

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.MapGet("/", () => "SignalR app");

            app.MapHub<ChatHub>("chat");
            app.Run();
        }
    }
}