using System;
using System.Collections.Generic;
using ClientManager.Models;

namespace ClientManager
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var clients = GenerateClients();

            Console.ReadKey();

            DisposeClients(clients);
        }


        // SUPPORT FUNCTIONS //////////////////////////////////////////////////////////////////////
        private static GameClient[] GenerateClients()
        {
            var roomsIds = new List<Guid>
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()
            };

            var clients = new List<GameClient>(30);
            for (var i = 1; i <= 2; i++)
            {
                var client = new GameClient(new ClientConfig
                {
                    RoomId = roomsIds[0],
                    NickName = $"Client {i}"
                });
                client.Start();
                clients.Add(client);
            }
            for (var i = 3; i <= 3; i++)
            {
                var client = new GameClient(new ClientConfig
                {
                    RoomId = roomsIds[1],
                    NickName = $"Client {i}"
                });
                client.Start();
                clients.Add(client);
            }

            return clients.ToArray();
        }

        private static void DisposeClients(IEnumerable<GameClient> clients)
        {
            foreach (var x in clients)
                x.Dispose();
        }
    }
}