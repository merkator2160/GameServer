using System;

namespace ClientManager
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new GameClient();
            client.Start();
            Console.WriteLine($"Client is runing: {client.Id}");
            Console.ReadKey();
        }
    }
}