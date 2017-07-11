using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common.Extensions;
using Common.Models;
using GameServer.Models;

namespace GameServer
{
    public class Room : IDisposable
    {
        private readonly Thread _chatThread;
        private readonly List<RoomMember> _members;
        private bool _disposed;


        public Room(Guid id)
        {
            Id = id;
            LastActivityDate = DateTime.UtcNow;
            _members = new List<RoomMember>();
            _chatThread = new Thread(BeginChatting);
            _chatThread.Start();
        }


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public Guid Id { get; set; }

        public int NumberOfMembers
        {
            get
            {
                lock (_members)
                {
                    return _members.Count();
                }
            }
        }

        public DateTime LastActivityDate { get; set; }


        // IDisposable ////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        public void AddParticipiant(RoomMember newMember)
        {
            lock (_members)
            {
                _members.Add(newMember);
            }
        }

        private void BeginChatting()
        {
            while (true)
                try
                {
                    lock (_members)
                    {
                        foreach (var x in _members)
                        {
                            if (!x.Client.Connected)
                            {
                                Console.WriteLine($"Client {x.Id} disconnected");
                                _members.Remove(x);
                                continue;
                            }

                            if (x.Stream.DataAvailable)
                            {
                                LastActivityDate = DateTime.UtcNow;

                                var receivedMessage = x.Stream.ReadObject<Message>();
                                SendMessageToNeighbors(receivedMessage, x);
                            }
                        }
                    }

                    Thread.Sleep(10);
                }
                catch (ThreadAbortException)
                {
                    //TODO: Sometimes occur when Thread disposing, maybe investigation required
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{nameof(BeginChatting)} something is broken: {ex.Message}");
#if DEBUG
                    throw;
#endif
                }
        }

        private void SendMessageToNeighbors(Message message, RoomMember current)
        {
            var roommates = _members.Except(new[] {current});
            foreach (var x in roommates)
                x.Stream.WriteObject(message);
        }

        private void SendEcho(Message message, RoomMember current)
        {
            current.Stream.WriteObject(message);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                ReleaseUnmanagedResources();
                if (disposing)
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
            _chatThread?.Abort();
            DisposeParticipiants();
        }

        private void DisposeParticipiants()
        {
            lock (_members)
            {
                foreach (var x in _members)
                    x.Client.Close();
            }
        }
    }
}