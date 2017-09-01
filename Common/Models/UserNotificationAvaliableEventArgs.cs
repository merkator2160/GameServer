using System;

namespace Common.Models
{
    public class UserNotificationAvaliableEventArgs : EventArgs
    {
        public UserNotificationAvaliableEventArgs(String message)
        {
            Message = message;
        }


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public String Message { get; }
    }
}