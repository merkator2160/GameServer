using Common.Extensions;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GameServer.Models
{
    public class Room : IDisposable
    {
        private readonly Thread _chatThread;
        private Boolean _disposed;
        private readonly List<RoomMember> _participiants;


        public Room(Guid id)
        {
            Id = id;
            LastActivityDate = DateTime.UtcNow;
            _participiants = new List<RoomMember>();
            _chatThread = new Thread(BeginChatting);
            _chatThread.Start();
        }


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public Guid Id { get; set; }
        public DateTime LastMessageDate { get; set; }
        public DateTime LastActivityDate { get; set; }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        public void AddParticipiant(RoomMember newMember)
        {
            lock (_participiants)
                _participiants.Add(newMember);
        }
        private void BeginChatting()
        {
            while (true)
            {
                try
                {
                    lock (_participiants)
                    {
                        foreach (var x in _participiants)
                        {
                            if (!x.Client.Connected)
                            {
                                Console.WriteLine($"Client {x.Id} disconnected");
                                _participiants.Remove(x);
                                continue;
                            }

                            if (x.Stream.DataAvailable)
                            {
                                LastActivityDate = DateTime.UtcNow;

                                var receivedMessage = x.Stream.ReadObject<Message>();
                                SendEcho(receivedMessage.Body, x);
                            }
                        }
                    }
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
        }
        private void SendMessageToNeighbors(String message, RoomMember current)
        {
            var roommates = _participiants.Except(new[] { current });
            foreach (var x in roommates)
            {
                x.Stream.WriteObject(new Message()
                {
                    Body = message
                });
            }
        }
        private void SendEcho(String message, RoomMember current)
        {
            current.Stream.WriteObject(new Message()
            {
                Body = $"Echo: {message}"
            });
        }


        // IDisposable ////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(Boolean disposing)
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
            lock (_participiants)
            {
                foreach (var x in _participiants)
                {
                    x.Client.Close();
                }
            }
        }
    }
}