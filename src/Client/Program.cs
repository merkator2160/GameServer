using System;
using System.Collections.Generic;

namespace ClientManager
{
    class Program
    {
        private static Int32 NumberOfClients = 10;

        static void Main(String[] args)
        {
            var clients = new List<GameClient>(NumberOfClients);
            for (int i = 0; i < NumberOfClients; i++)
            {
                var client = new GameClient();
                client.Start();
                Console.WriteLine($"Client {client.Id}: Is started");
                clients.Add(client);
            }

            Console.ReadKey();

            foreach (var x in clients)
            {
                x.Dispose();
            }
        }
    }
}