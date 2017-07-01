using ProtoBuf;
using System;

namespace Common.Models
{
    [Serializable]
    [ProtoContract]
    public class Message
    {
        [ProtoMember(1)]
        public String Body { get; set; }
    }
}