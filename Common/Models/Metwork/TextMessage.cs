using System;

namespace Common.Models.Metwork
{
    [Serializable]
    public class TextMessage
    {
        public String Text { get; set; }
        public String From { get; set; }
    }
}