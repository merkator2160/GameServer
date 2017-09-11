using System;

namespace Client.Models
{
    internal class NetworkConfig
    {
        public String Host { get; set; }
        public Int32 Port { get; set; }
        public Int32 ReconnectDelay { get; set; }
    }
}