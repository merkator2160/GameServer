using System;
using System.Net.Sockets;

namespace GameServer.Models
{
    public class RoomParticipiant
    {
        public Guid Id { get; set; }
        public NetworkStream Stream { get; set; }
    }
}