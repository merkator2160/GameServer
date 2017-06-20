using Common.Extensions;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.Models
{
    public class Room : IDisposable
    {
        private CancellationTokenSource _chattingCancelationTokenSource;
        private bool _disposed;
        private readonly List<RoomMember> _participiants;


        public Room(Guid id)
        {
            Id = id;
            _participiants = new List<RoomMember>();
            BeginChatting();
        }


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public Guid Id { get; set; }
        public DateTime LastMessageDate { get; set; }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        public void AddParticipiant(RoomMember newMember)
        {
            lock(_participiants)
                _participiants.Add(newMember);
        }
        private void BeginChatting()
        {
            using(_chattingCancelationTokenSource = new CancellationTokenSource())
            {
                var token = _chattingCancelationTokenSource.Token;
                Task.Factory.StartNew(() =>
                {
                    while(true)
                    {
                        try
                        {
                            if(token.IsCancellationRequested)
                                return;

                            lock(_participiants)
                            {
                                foreach(var x in _participiants)
                                {
                                    if(x.Stream.DataAvailable)
                                    {
                                        var receivedMessage = x.Stream.ReadObject<Message>();
                                        SendMessageToNeighbors(receivedMessage.Body, x);
                                    }
                                }
                            }

                            Thread.Sleep(10);
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex);
#if DEBUG
                            throw;
#endif
                        }
                    }
                }, token);
            }
        }
        private void SendMessageToNeighbors(string message, RoomMember current)
        {
            var roommates = _participiants.Except(new[] { current });
        }


        // IDisposable ////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
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

        }
        private void ReleaseManagedResources()
        {
            _chattingCancelationTokenSource?.Cancel();
            _chattingCancelationTokenSource?.Dispose();
        }
    }
}