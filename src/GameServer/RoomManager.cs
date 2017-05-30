using Common.Extensions;
using Common.Models;
using GameServer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer
{
    public class RoomManager
    {
        private List<Room> _rooms;
        private CancellationTokenSource _cancelTokenSource;


        public RoomManager()
        {
            _rooms = new List<Room>();
        }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        public void AcceptClient(NetworkStream stream)
        {
            var request = stream.Read<ConnectionRequest>();
            var requestedRoom = _rooms.FirstOrDefault(r => r.Id == request.RoomId);
            if (requestedRoom == null)
            {
                var newRoom = new Room(request.RoomId);
                newRoom.Participiants.Add(new RoomParticipiant()
                {
                    Id = request.ClientId,
                    Stream = stream
                });
                _rooms.Add(newRoom);
            }
            else
            {
                requestedRoom.Participiants.Add(new RoomParticipiant()
                {
                    Id = request.ClientId,
                    Stream = stream
                });
            }
        }
        private void RunTalk()
        {
            using (_cancelTokenSource = new CancellationTokenSource())
            {
                var token = _cancelTokenSource.Token;
                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        foreach (var room in _rooms)
                        {
                            foreach (var roomParticipiant in room.Participiants)
                            {
                                var receivedMessage = roomParticipiant.Stream.Read<Message>();
                            }
                        }
                    }
                }, token);
            }
        }
    }
}