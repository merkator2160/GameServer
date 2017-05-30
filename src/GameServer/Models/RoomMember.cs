using System;
using System.Net.Sockets;

namespace GameServer.Models
{
    public class RoomMember
    {
        public Guid Id { get; set; }
        public NetworkStream Stream { get; set; }
    }
}