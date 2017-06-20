using ProtoBuf;
using System;

namespace Common.Models
{

    [Serializable]
    [ProtoContract]
    public class TestClass
    {
        [ProtoMember(1)]
        public String Str { get; set; }
    }
}