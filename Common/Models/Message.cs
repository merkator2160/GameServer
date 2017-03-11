using ProtoBuf;
using System;

namespace Common.Models
{
    [Serializable]
    [ProtoContract]
    public class Message
    {
        [ProtoMember(1)]
        public Guid RoomId { get; set; }

        [ProtoMember(2)]
        public String Body { get; set; }
    }
}