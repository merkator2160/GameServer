using ProtoBuf;
using System;

namespace Common.Models
{
    [Serializable]
    [ProtoContract]
    public class Message
    {
        [ProtoMember(1)]
        public string Body { get; set; }
    }
}