using ProtoBuf;
using System;

namespace Common.Models
{
    [Serializable]
    [ProtoContract]
    public class ConnectionRequest
    {
        [ProtoMember(1)]
        public Guid ClientId { get; set; }

        [ProtoMember(2)]
        public Guid RoomId { get; set; }
    }
}