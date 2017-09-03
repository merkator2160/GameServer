using System.Net.Sockets;

namespace Common.Models.MvvmLight
{
    public class NewClientAvaliableMessage
    {
        public NewClientAvaliableMessage(TcpClient client)
        {
            Client = client;
        }


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public TcpClient Client { get; set; }
    }
}