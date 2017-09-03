using System;
using Common.Models.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace Server
{
    internal class ServerConsoleWriter
    {
        public ServerConsoleWriter(IMessenger messenger)
        {
            messenger.Register<ConsoleMessage>(this, DisplayMessage);
        }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        private void DisplayMessage(ConsoleMessage consoleMessage)
        {
            Console.Clear();
            Console.WriteLine(consoleMessage.Text);
        }
    }
}