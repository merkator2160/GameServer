using System;

namespace Server.Models
{
    public class RootConfig
    {
        public Int32 Port { get; set; }
        public RoomManagerConfig RoomManagerConfig { get; set; }
    }
}