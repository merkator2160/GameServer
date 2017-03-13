using System;
using System.Collections.Generic;

namespace GameServer.Models
{
    public class Room
    {
        public Room(Guid id)
        {
            Id = id;
            Participiants = new List<RoomParticipiant>();
        }


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public Guid Id { get; set; }
        public DateTime LastMessageDate { get; set; }
        public List<RoomParticipiant> Participiants { get; set; }
    }
}