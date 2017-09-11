﻿using System;

namespace Common.Models.Metwork
{
    [Serializable]
    public class ConnectionRequest
    {
        public String NickName { get; set; }
        public SerializableGuid RoomId { get; set; }
        public SerializableGuid SessionId { get; set; }
    }
}