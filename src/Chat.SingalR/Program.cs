namespace Chat.SingalR
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSignalR();

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.MapGet("/", () => "SignalR app");

            app.Run();
        }
    }
}