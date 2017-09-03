using Common;
using Common.Models.Metwork;
using Common.Models.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    internal class Room : IDisposable
    {
        private Boolean _disposed;
        private readonly List<RoomMember> _members;
        private readonly IMessenger _roomMessenger;

        public Room(Guid id)
        {
            Id = id;
            LastActivityDate = DateTime.UtcNow;
            _members = new List<RoomMember>();
            _roomMessenger = new Messenger();
            _roomMessenger.Register<TextReceiveMessage>(this, RoomMessageTextReceived);

            ThreadPool.QueueUserWorkItem(UserCleaningThread);
        }


        // THREADS ////////////////////////////////////////////////////////////////////////////////
        private void UserCleaningThread(Object state)
        {
            while (!_disposed)
            {
                try
                {
                    lock (_members)
                    {
                        RemoveDisconnectedMembers();
                    }

                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw;
                }
            }
        }
        private void RemoveDisconnectedMembers()
        {
            var disconnectedMembers = _members.Where(p => !p.Client.Connected).ToArray();
            foreach (var x in disconnectedMembers)
            {
                _members.Remove(x);
                x.Dispose();
            }
        }


        // HANDLERS ///////////////////////////////////////////////////////////////////////////////
        private void RoomMessageTextReceived(TextReceiveMessage obj)
        {
            LastActivityDate = DateTime.UtcNow;
        }


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public Guid Id { get; set; }
        public Int32 NumberOfMembers
        {
            get
            {
                lock (_members)
                {
                    return _members.Count();
                }
            }
        }
        public DateTime LastActivityDate
        {
            get => DateTime.FromBinary(Interlocked.Read(ref _lastActivityDate));
            set => Interlocked.Exchange(ref _lastActivityDate, value.ToBinary());
        }
        private Int64 _lastActivityDate;


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        public void AddParticipiant(TcpClient client, Guid clientId)
        {
            var bufferedClient = new BufferedTcpClient<NetworkMessage>(client, true);
            lock (_members)
            {
                _members.Add(new RoomMember(clientId, bufferedClient, _roomMessenger, false));
            }
        }


        // IDisposable ////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                lock (_members)
                {
                    foreach (var x in _members)
                    {
                        x.Dispose();
                    }
                }
            }
        }
    }
}