using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer
{
    class Program
    {
        private const String Host = "127.0.0.1";
        private const Int32 Port = 8888;

        private static TcpListener _tcpListener;
        private static RoomManager _roomManager;
        private static CancellationTokenSource _cancelTokenSource;

        static void Main(String[] args)
        {
            CheckAnyOtherInstances();

            _roomManager = new RoomManager();
            _tcpListener = new TcpListener(IPAddress.Parse(Host), Port);
            _tcpListener.Start();

            using(_cancelTokenSource = new CancellationTokenSource())
            {
                var token = _cancelTokenSource.Token;
                WitingForClients(token);

                Console.ReadKey();
                _cancelTokenSource.Cancel();
            }

            _tcpListener.Stop();
        }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////////
        private static void CheckAnyOtherInstances()
        {
            bool created;
            var guid = Marshal.GetTypeLibGuidForAssembly(Assembly.GetExecutingAssembly()).ToString();
            var mutexObj = new Mutex(true, guid, out created);

            if(!created)
            {
                Console.WriteLine("Application instance already exist");
                Thread.Sleep(3000);
                Environment.Exit(0);
            }
        }
        private static void WitingForClients(CancellationToken token)
        {
            Task.Factory.StartNew(() =>
            {
                while(true)
                {
                    if(token.IsCancellationRequested)
                        break;

                    if(!_tcpListener.Pending())
                    {
                        Thread.Sleep(10);
                        return;
                    }

                    using(var tcpClient = _tcpListener.AcceptTcpClient())
                    {
                        try
                        {
                            _roomManager.AcceptClient(tcpClient.GetStream());
                        }
                        catch(IOException)
                        {

                        }
                        catch(Exception ex)
                        {
                            Console.Clear();
                            Console.WriteLine("Something is broken:");
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }, token);
        }
    }
}
