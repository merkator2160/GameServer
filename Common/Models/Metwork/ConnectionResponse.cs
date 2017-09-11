using System;

namespace Common.Models.Metwork
{

    [Serializable]
    public class ConnectionResponse
    {
        public SerializableGuid SessionId { get; set; }
    }
}