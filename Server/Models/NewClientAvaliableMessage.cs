using System.Net.Sockets;

namespace Server.Models
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