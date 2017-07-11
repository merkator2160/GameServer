using System;

namespace ClientManager.Models
{
    public class ClientConfig
    {
        private const String DefaultHost = "127.0.0.1";
        private const Int32 DefaultPort = 8888;
        public const Int32 SendMessageDelay = 100;
        public const Int32 ReconnectDelay = 3000;
        public const Int32 ReceiveOperationsTimeout = 100;
        public const Int32 SendOperationsTimeout = 100;


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
        public String NickName { get; set; }
        public String Host { get; set; }
        public Int32 Port { get; set; }
        public Guid ClientId { get; set; }
        public Guid RoomId { get; set; }
        public Boolean IsWritingEcho { get; set; }
    }
}