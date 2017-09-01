using System;
using System.Net.Sockets;

namespace GameServer.Models
{
    internal class NewClientAvaliableEventArgs : EventArgs
    {
        public NewClientAvaliableEventArgs(TcpClient client)
        {
            Client = client;
        }


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public TcpClient Client { get; }
    }
}