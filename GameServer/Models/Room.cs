using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace GameServer.Models
{
    public class Room
    {
        public Room()
        {
            Id = Guid.NewGuid();
            Clients = new List<NetworkStream>();
        }
        public Room(Guid id)
        {
            Clients = new List<NetworkStream>();
        }


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public Guid Id { get; set; }
        public List<NetworkStream> Clients { get; set; }
    }
}