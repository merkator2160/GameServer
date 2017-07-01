using System;

namespace Common.Models
{
    [Serializable]
    public class ConnectionRequest
    {
        public SerializableGuid ClientId { get; set; }
        public SerializableGuid RoomId { get; set; }
    }
}