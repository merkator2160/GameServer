using System;

namespace Client.Models
{
    internal class NumberGeneratedMessage
    {
        public NumberGeneratedMessage(Int64 number)
        {
            Number = number;
        }


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public Int64 Number { get; set; }
    }
}