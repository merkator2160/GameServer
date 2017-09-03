using Common.Extensions;
using Common.Models.Metwork;
using Common.Models.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Server.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    internal class RoomManager : IDisposable
    {
        private readonly IMessenger _applicationMessenger;

        private Boolean _disposed;
        private readonly List<Room> _chatRooms;
        private readonly RoomManagerConfig _config;
        private readonly ConcurrentQueue<TcpClient> _newClientQueue;


        public RoomManager(ServerConfig config, IMessenger messenger)
        {
            _applicationMessenger = messenger;

            _config = config.RoomManagerConfig;
            _chatRooms = new List<Room>();
            _newClientQueue = new ConcurrentQueue<TcpClient>();

            messenger.Register<NewClientAvaliableMessage>(this, PutNewClientToQueue);

            ThreadPool.QueueUserWorkItem(RoomCleanerThread);
            ThreadPool.QueueUserWorkItem(NewClientProcessingThread);
        }


        // THREADS ////////////////////////////////////////////////////////////////////////////////
        private void RoomCleanerThread(Object state)
        {
            while (!_disposed)
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
                    Debug.WriteLine(ex.Message);
                }
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

                _applicationMessenger.Send(new ConsoleMessage($"Room {x.Id} closed due inactivity"));
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
            _applicationMessenger.Send(new ConsoleMessage(strBuilder.ToString()));
        }

        private void NewClientProcessingThread(Object state)
        {
            while (!_disposed)
            {
                try
                {
                    if (_newClientQueue.IsEmpty)
                    {
                        Thread.Sleep(10);
                        continue;
                    }

                    HandleNewClient();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw;
                }
            }
        }
        private void HandleNewClient()
        {
            if (_newClientQueue.TryDequeue(out TcpClient client))
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

                    requestedRoom.AddParticipiant(client, request.ClientId);
                }
            }
        }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        public void PutNewClientToQueue(NewClientAvaliableMessage message)
        {
            _newClientQueue.Enqueue(message.Client);
        }


        // IDisposable ////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                foreach (var x in _chatRooms)
                {
                    x.Dispose();
                }
            }
        }
    }
}