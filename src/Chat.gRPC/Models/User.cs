using Grpc.Core;

namespace Chat.gRPC.Models
{
    public class User
    {
        public string Name { get; set; }

        public IServerStreamWriter<ServerMessage> StreamWriter { get; set; }

        public User(string name, IServerStreamWriter<ServerMessage> streamWriter)
        {
            Name = name;
            StreamWriter = streamWriter;
        }
    }
}
