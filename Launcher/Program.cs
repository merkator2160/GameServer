using System.Diagnostics;
using System.Threading;

namespace Launcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var serverProcess = new Process
            {
                StartInfo =
                {
                    FileName = "Server.exe"
                }
            };
            serverProcess.Start();

            Thread.Sleep(1000);

            var clientProcess = new Process
            {
                StartInfo =
                {
                    FileName = "Client.exe",
                    Arguments = "127.0.0.1 8888 16f7aba5-606d-42fa-8dec-f19aa9dea66a 331031b3-1e27-4aa0-943a-df39178e6063"
                }
            };
            clientProcess.Start();

            Thread.Sleep(5000);

            var clientProcess2 = new Process
            {
                StartInfo =
                {
                    FileName = "Client.exe",
                    Arguments = "127.0.0.1 8888 f126be5d-8982-4592-9da1-c2ce4ddfcd30 331031b3-1e27-4aa0-943a-df39178e6063"
                }
            };
            clientProcess2.Start();
        }
    }
}
