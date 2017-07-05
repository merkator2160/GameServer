using Common.Extensions;
using Common.Models;
using GameServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace GameServer
{
    public class RoomManager : IDisposable
    {
        private Boolean _disposed;
        private readonly List<Room> _chatRooms;
        private readonly Thread _roomCleanUpThread;
        private readonly RoomManagerConfig _config;


        public RoomManager(RoomManagerConfig config)
        {
            _config = config;
            _chatRooms = new List<Room>();
            _roomCleanUpThread = new Thread(RoomCleanUp);
            _roomCleanUpThread.Start();
        }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        public void AcceptClient(TcpClient client)
        {
            var stream = client.GetStream();
            var request = stream.ReadObject<ConnectionRequest>();

            lock(_chatRooms)
            {
                var requestedRoom = _chatRooms.FirstOrDefault(r => r.Id == request.RoomId);
                if(requestedRoom == null)
                {
                    requestedRoom = new Room(request.RoomId);
                    _chatRooms.Add(requestedRoom);
                }

                requestedRoom.AddParticipiant(new RoomMember()
                {
                    Id = request.ClientId,
                    Client = client
                });
            }
        }
        private void RoomCleanUp()
        {
            while(true)
            {
                try
                {
                    lock(_chatRooms)
                    {
                        var obsoleteRooms = CollectObsoleteRooms();
                        DeleteObsoleteRooms(obsoleteRooms);
                        PrintReport();
                    }

                    Thread.Sleep(_config.CleanUpThreadIdle);
                }
                catch(ThreadAbortException)
                {
                    //TODO: Sometimes occur when Thread disposing, maybe investigation required
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"{nameof(RoomCleanUp)} something is broken: {ex.Message}");
#if DEBUG
                    throw;
#endif
                }
            }
        }
        private Room[] CollectObsoleteRooms()
        {
            return (from x in _chatRooms
                    let delitingRoomThreshold = DateTime.UtcNow - _config.EmptyRoomLifeTime
                    where delitingRoomThreshold > x.LastActivityDate
                    select x).ToArray();
        }
        private void DeleteObsoleteRooms(Room[] obsoleteRooms)
        {
            foreach(var x in obsoleteRooms)
            {
                _chatRooms.Remove(x);
                x.Dispose();
            }
        }
        private void PrintReport()
        {
            Console.Clear();
            Console.WriteLine($"Rooms: {_chatRooms.Count}");
            Console.WriteLine();
            foreach(var x in _chatRooms)
            {
                var lifeTimeLeft = x.LastActivityDate - (DateTime.UtcNow - _config.EmptyRoomLifeTime);
                Console.WriteLine($"Room {x.Id}, member count: {x.NumberOfMembers}, life time left: {lifeTimeLeft.Seconds} sec");
            }
        }


        // IDisposable ////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(Boolean disposing)
        {
            if(!_disposed)
            {
                ReleaseUnmanagedResources();
                if(disposing)
                    ReleaseManagedResources();

                _disposed = true;
            }
        }
        private void ReleaseUnmanagedResources()
        {
            // We didn't have its yet.
        }
        private void ReleaseManagedResources()
        {
            _roomCleanUpThread?.Abort();
        }
    }
}