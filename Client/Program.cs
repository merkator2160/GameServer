using Client.Models;
using System;

namespace Client
{
    static class Program
    {
        static void Main(String[] args)
        {
            var client = new GameClient(CreateConfigFromCommandArgs(args));
            client.UserNotificationAvaliable += (sender, eventArgs) => Console.WriteLine(eventArgs.Message);

            client.Start();

            Console.ReadKey();

            client.Dispose();
        }
        private static ClientConfig CreateConfigFromCommandArgs(String[] args)
        {
            var host = args.Length > 0 ? args[0] : "127.0.0.1";
            var port = args.Length > 1 ? Int32.Parse(args[1]) : 8888;
            var clientId = args.Length > 2 ? Guid.Parse(args[2]) : Guid.NewGuid();
            var roomId = args.Length > 3 ? Guid.Parse(args[3]) : Guid.NewGuid();

            return new ClientConfig(host, port, clientId, roomId);
        }
    }
}