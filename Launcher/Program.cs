using System;
using System.Diagnostics;
using System.Threading;

namespace Launcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var room1 = "331031b3-1e27-4aa0-943a-df39178e6063";
            var room2 = "043659ee-3e10-4c9c-a533-217f9e30c691";
            var room3 = "4b9289b2-23fd-41af-902b-890450873083";

            RunServer(8888);

            Thread.Sleep(1000);

            RunClient("16f7aba5-606d-42fa-8dec-f19aa9dea66a", room1);
            //RunClient("f126be5d-8982-4592-9da1-c2ce4ddfcd30", room1);
            //RunClient("35bdec8f-fe9b-4706-bcc6-739555e1f0bf", room1);
            //RunClient("454fce89-a69d-423d-a8d7-36e834c7a378", room1);

            //RunClient("349661cd-46b7-4b9a-89d7-7d0e4fc16adf", room2);

            //RunClient("c2f103af-0a5c-4d2b-b70d-2aa2bc2c2765", room3);
            //RunClient("b0faad8c-2bd2-4b67-ae7f-d27ca9212c1d", room3);
            //RunClient("be089750-7360-4724-97f6-5f2b687849e9", room3);
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
        private static void RunClient(String clientId, String roomId)
        {
            var clientProcess = new Process
            {
                StartInfo =
                {
                    FileName = "Client.exe",
                    Arguments = $"127.0.0.1 8888 {clientId} {roomId}"
                }
            };
            clientProcess.Start();
        }
    }
}
