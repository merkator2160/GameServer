using Common.Models.Enums;
using System;

namespace Common.Models.Metwork
{
    [Serializable]
    public class NetworkMessage
    {
        public MessageType Type { get; set; }
        public Byte[] Data { get; set; }
    }
}