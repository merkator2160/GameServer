using System;

namespace Client.Models
{
    internal class RootConfig
    {
        public String NickName { get; set; }
        public Guid RoomId { get; set; }
        public NetworkConfig NetworkConfig { get; set; }
        public NumberGeneratorConfig NumberGeneratorConfig { get; set; }
    }
}