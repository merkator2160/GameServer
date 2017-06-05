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


        public Room(Guid id)
        {
            Id = id;
            Participiants = new List<RoomMember>();
        }


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public Guid Id { get; set; }
        public DateTime LastMessageDate { get; set; }
        public List<RoomMember> Participiants { get; set; }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
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

                            foreach(var x in Participiants)
                            {
                                if(x.Stream.DataAvailable)
                                {
                                    var receivedMessage = x.Stream.Read<Message>();
                                    SendMessageToOtherParticipiants(receivedMessage.Body, x);
                                }
                            }
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
        private void SendMessageToOtherParticipiants(string message, RoomMember current)
        {
            var roommates = Participiants.Except(new[] { current });
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