using Common.Models.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;

namespace Client
{
    internal class ConsoleWriter
    {
        public ConsoleWriter(IMessenger messenger)
        {
            messenger.Register<ConsoleMessage>(this, DisplayMessage);
        }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        private void DisplayMessage(ConsoleMessage consoleMessage)
        {
            Console.WriteLine(consoleMessage.Text);
        }
    }
}