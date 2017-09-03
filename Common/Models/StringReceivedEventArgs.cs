using System;

namespace Common.Models
{
    public class StringReceivedEventArgs : EventArgs
    {
        public StringReceivedEventArgs(String text)
        {
            Text = text;
        }


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public String Text { get; }
    }
}