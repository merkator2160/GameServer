using System;

namespace Client.Models
{
    internal class ClientConfig
    {

        public const Int32 DefaultSendMessageDelay = 100;
        public const Int32 DefaultReconnectDelay = 3000;


        public ClientConfig(String host, Int32 port, Guid clientId, Guid roomId)
        {
            Host = host;
            Port = port;
            ClientId = clientId;
            RoomId = roomId;
            ReconnectDelay = DefaultReconnectDelay;
            SendMessageDelay = DefaultSendMessageDelay;
        }


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public String Host { get; set; }
        public Int32 Port { get; set; }
        public Guid ClientId { get; set; }
        public Guid RoomId { get; set; }
        public Int32 SendMessageDelay { get; set; }
        public Int32 ReconnectDelay { get; set; }
    }
}