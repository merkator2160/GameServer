using System;

namespace ClientManager
{
    class Program
    {
        static void Main(String[] args)
        {
            var client = new GameClient(Guid.NewGuid(), Guid.NewGuid());
            client.Run();
            Console.WriteLine($"Client is runing: {client.Id}");
        }
    }
}