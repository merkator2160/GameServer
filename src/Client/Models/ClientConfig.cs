using System;

namespace ClientManager.Models
{
    public class ClientConfig
    {
        private const string DefaultHost = "127.0.0.1";
        private const int DefaultPort = 8888;
        public const int SendMessageDelay = 100;
        public const int ReceiveMessageDelay = 100;
        public const int SendReceiveOperationsTimeout = 3000;


        public ClientConfig() : this(DefaultHost, DefaultPort, Guid.NewGuid(), Guid.NewGuid())
        {

        }
        public ClientConfig(string host, int port, Guid clientId, Guid roomId)
        {
            Host = host;
            Port = port;
            ClientId = clientId;
            RoomId = roomId;
        }


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public string Host { get; set; }
        public int Port { get; set; }
        public Guid ClientId { get; set; }
        public Guid RoomId { get; set; }
    }
}