using System;

namespace Client.Models
{
    internal class Config
    {
        public const Int32 DefaultReconnectDelay = 3000;


        public Config(String host, Int32 port, Guid clientId, Guid roomId)
        {
            Host = host;
            Port = port;
            ClientId = clientId;
            RoomId = roomId;
            ReconnectDelay = DefaultReconnectDelay;
        }


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public String Host { get; set; }
        public Int32 Port { get; set; }
        public Guid ClientId { get; set; }
        public Guid RoomId { get; set; }
        public Int32 ReconnectDelay { get; set; }
    }
}