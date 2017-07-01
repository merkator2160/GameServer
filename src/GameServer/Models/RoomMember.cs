using System;
using System.Net.Sockets;

namespace GameServer.Models
{
    public class RoomMember
    {
        public Guid Id { get; set; }
        public TcpClient Client { get; set; }
        public NetworkStream Stream => Client.GetStream();
    }
}