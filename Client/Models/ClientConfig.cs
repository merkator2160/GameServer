using System;

namespace ClientManager.Models
{
    public class ClientConfig
    {
        private const string DefaultHost = "127.0.0.1";
        private const int DefaultPort = 8888;
        public const int SendMessageDelay = 100;
        public const int ReconnectDelay = 3000;
        public const int ReceiveOperationsTimeout = 100;
        public const int SendOperationsTimeout = 100;


        public ClientConfig()
        {
            Host = DefaultHost;
            Port = DefaultPort;

            var clientId = Guid.NewGuid();
            ClientId = clientId;
            NickName = clientId.ToString();

            RoomId = Guid.NewGuid();
        }


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public string NickName { get; set; }

        public string Host { get; set; }
        public int Port { get; set; }
        public Guid ClientId { get; set; }
        public Guid RoomId { get; set; }
        public bool IsWritingEcho { get; set; }
    }
}