using System;

namespace Common.Models.MvvmLight
{
    public class TextReceiveMessage
    {
        public TextReceiveMessage(String text, Guid sender)
        {
            Text = text;
            Sender = sender;
        }


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public Guid Sender { get; set; }
        public String Text { get; set; }
    }
}