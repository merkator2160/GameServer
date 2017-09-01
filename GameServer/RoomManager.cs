using Common;
using Common.Extensions;
using Common.Models;
using GameServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GameServer
{
    internal class RoomManager : IDisposable
    {
        private readonly List<Room> _chatRooms;
        private readonly RoomManagerConfig _config;


        public RoomManager(RoomManagerConfig config)
        {
            _config = config;
            _chatRooms = new List<Room>();

            ThreadPool.QueueUserWorkItem(RoomCleanUpThread);
        }


        public event UserNotificationAvaliableEventHandler UserNotificationAvaliable = (sender, args) => { };


        // THREADS ////////////////////////////////////////////////////////////////////////////////
        private void RoomCleanUpThread(Object state)
        {
            while (true)
            {
                try
                {
                    lock (_chatRooms)
                    {
                        var obsoleteRooms = FindObsoleteRooms();
                        DeleteObsoleteRooms(obsoleteRooms);
                        PrintReport();
                    }

                    Thread.Sleep(_config.CleanUpThreadIdle);
                }
                catch (Exception ex)
                {
                    UserNotificationAvaliable.Invoke(this, new UserNotificationAvaliableEventArgs(ex.Message));
                    throw;
                }
            }
        }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        public void AcceptClient(TcpClient client)
        {
            var stream = client.GetStream();
            var request = stream.ReadObject<ConnectionRequest>();

            lock (_chatRooms)
            {
                var requestedRoom = _chatRooms.FirstOrDefault(r => r.Id == request.RoomId);
                if (requestedRoom == null)
                {
                    requestedRoom = new Room(request.RoomId);
                    _chatRooms.Add(requestedRoom);
                }

                requestedRoom.AddParticipiant(new BufferedClient<Message>(request.ClientId, client));
            }
        }
        private Room[] FindObsoleteRooms()
        {
            return (from x in _chatRooms
                    let delitingRoomThreshold = DateTime.UtcNow - _config.EmptyRoomLifeTime
                    where delitingRoomThreshold > x.LastActivityDate
                    select x).ToArray();
        }
        private void DeleteObsoleteRooms(Room[] obsoleteRooms)
        {
            foreach (var x in obsoleteRooms)
            {
                _chatRooms.Remove(x);
                x.Dispose();
                UserNotificationAvaliable.Invoke(this, new UserNotificationAvaliableEventArgs($"Room {x.Id} closed due inactivity"));
            }
        }
        private void PrintReport()
        {
            var strBuilder = new StringBuilder();

            strBuilder.AppendLine($"Rooms: {_chatRooms.Count}");
            strBuilder.AppendLine();
            strBuilder.AppendLine();

            foreach (var x in _chatRooms)
            {
                var lifeTimeLeft = x.LastActivityDate - (DateTime.UtcNow - _config.EmptyRoomLifeTime);
                strBuilder.AppendLine($"Room {x.Id}, member count: {x.NumberOfMembers}, life time left: {lifeTimeLeft.Seconds} sec");
            }

            UserNotificationAvaliable.Invoke(this, new UserNotificationAvaliableEventArgs(strBuilder.ToString()));
        }


        // IDisposable ////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            foreach (var x in _chatRooms)
            {
                x.Dispose();
            }
        }
    }
}