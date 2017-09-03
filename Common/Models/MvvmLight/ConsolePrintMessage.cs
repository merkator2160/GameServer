using System;

namespace Common.Models.MvvmLight
{
    public class ConsoleMessage
    {
        public ConsoleMessage(String text)
        {
            Text = text;
        }


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public String Text { get; set; }
    }
}