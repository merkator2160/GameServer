using System;

namespace Common.Models
{
    [Serializable]
    public class Message
    {
        private SerializableGuid ClientId;
        public String Body { get; set; }
    }
}