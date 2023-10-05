namespace Chat.SingalR.Hubs
{
    public interface IChat
    {
        Task SendMessageAsync(string message);

        Task LoginAsync(string username, string chatRoomId);
    }
}
