using System;

namespace Common.Models
{
    [Serializable]
    public class Message
    {
        public String From { get; set; }
        public String Body { get; set; }
    }
}