using Common.Extensions;
using Common.Models;
using GameServer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace GameServer
{
    public class RoomManager
    {
        private readonly List<Room> _chatRooms;



        public RoomManager()
        {
            _chatRooms = new List<Room>();
        }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        public void AcceptClient(NetworkStream stream)
        {
            var request = stream.Read<ConnectionRequest>();
            var requestedRoom = _chatRooms.FirstOrDefault(r => r.Id == request.RoomId);
            if(requestedRoom == null)
            {
                var newRoom = new Room(request.RoomId);
                newRoom.Participiants.Add(new RoomMember()
                {
                    Id = request.ClientId,
                    Stream = stream
                });
                _chatRooms.Add(newRoom);
            }
            else
            {
                requestedRoom.Participiants.Add(new RoomMember()
                {
                    Id = request.ClientId,
                    Stream = stream
                });
            }
        }
    }
}