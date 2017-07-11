using System;

namespace Common.Models
{
    [Serializable]
    public class Message
    {
        public string From { get; set; }
        public string Body { get; set; }
    }
}