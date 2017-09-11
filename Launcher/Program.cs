using System;
using System.Diagnostics;
using System.Threading;

namespace Launcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var rnd = new Random();
            var room1 = "331031b3-1e27-4aa0-943a-df39178e6063";
            var room2 = "043659ee-3e10-4c9c-a533-217f9e30c691";
            var room3 = "4b9289b2-23fd-41af-902b-890450873083";

            RunServer(8888);

            Thread.Sleep(1000);

            RunClient($"Nick_{rnd.Next()}", room1);
            RunClient($"Nick_{rnd.Next()}", room1);
            RunClient($"Nick_{rnd.Next()}", room1);
            RunClient($"Nick_{rnd.Next()}", room1);

            RunClient($"Nick_{rnd.Next()}", room2);

            RunClient($"Nick_{rnd.Next()}", room3);
            RunClient($"Nick_{rnd.Next()}", room3);
            RunClient($"Nick_{rnd.Next()}", room3);
        }
        private static void RunServer(Int32 port)
        {
            var serverProcess = new Process
            {
                StartInfo =
                {
                    FileName = "Server.exe",
                    Arguments = port.ToString()
                }
            };
            serverProcess.Start();
        }
        private static void RunClient(String nickName, String roomId)
        {
            var clientProcess = new Process
            {
                StartInfo =
                {
                    FileName = "Client.exe",
                    Arguments = $"127.0.0.1 8888 {nickName} {roomId}"
                }
            };
            clientProcess.Start();
        }
    }
}
