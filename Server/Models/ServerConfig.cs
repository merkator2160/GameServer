using System;

namespace Server.Models
{
    public class ServerConfig
    {
        public Int32 ListeningPort { get; set; }
        public RoomManagerConfig RoomManagerConfig { get; set; }
    }
}