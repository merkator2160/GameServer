using System;

namespace Common.Models.Metwork
{
    [Serializable]
    public class ConnectionRequest
    {
        public SerializableGuid ClientId { get; set; }
        public SerializableGuid RoomId { get; set; }
    }
}