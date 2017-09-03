using Common.Models.Enums;
using System;

namespace Common.Models.Metwork
{
    [Serializable]
    public class NetworkMessage
    {
        public NetworkMessage(MessageType type, Byte[] data)
        {
            Type = type;
            Data = data;
        }


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public MessageType Type { get; }
        public Byte[] Data { get; }
    }
}