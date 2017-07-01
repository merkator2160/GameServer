using System;

namespace GameServer.Models
{
    public class RoomManagerConfig
    {
        public TimeSpan CleanUpThreadIdle { get; set; }
        public TimeSpan EmptyRoomLifeTime { get; set; }
    }
}