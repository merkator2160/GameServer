using ClientManager.Models;
using System;
using System.Collections.Generic;

namespace ClientManager
{
    static class Program
    {
        static void Main(String[] args)
        {
            var roomsIds = new List<Guid>()
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()
            };

            var clients = new List<GameClient>(30);
            for (int i = 0; i < 2; i++)
            {
                var client = new GameClient(new ClientConfig()
                {
                    RoomId = roomsIds[0]
                });
                client.Start();
                clients.Add(client);
            }
            for (int i = 0; i < 1; i++)
            {
                var client = new GameClient(new ClientConfig()
                {
                    RoomId = roomsIds[1]
                });
                client.Start();
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