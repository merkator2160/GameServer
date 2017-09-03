using System;

namespace Common.Models
{
    public class ConnectionRequestReceivedEventArgs : EventArgs
    {
        public ConnectionRequestReceivedEventArgs(Guid clientId, Guid roomId)
        {
            ClientId = clientId;
            RoomId = roomId;
        }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        public Guid ClientId { get; }
        public Guid RoomId { get; }
    }
}