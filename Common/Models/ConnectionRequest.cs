using ProtoBuf;
using System;

namespace Common.Models
{
    [Serializable]
    [ProtoContract]
    public class ConnectionRequest
    {
        [ProtoMember(1)]
        public SerializableGuid ClientId { get; set; }

        [ProtoMember(2)]
        public SerializableGuid RoomId { get; set; }
    }
}