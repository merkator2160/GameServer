using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using GameServer.Models;

namespace GameServer
{
    internal static class Program
    {
        private const string Host = "127.0.0.1";
        private const int Port = 8888;

        private static TcpListener _tcpListener;
        private static Thread _clientWorkerThread;
        private static RoomManager _roomManager;


        private static void Main(string[] args)
        {
            CheckAnyOtherInstances();

            _roomManager = new RoomManager(new RoomManagerConfig
            {
                CleanUpThreadIdle = TimeSpan.FromSeconds(1),
                EmptyRoomLifeTime = TimeSpan.FromMinutes(1)
            });

            _tcpListener = new TcpListener(IPAddress.Parse(Host), Port);
            _tcpListener.Start();

            _clientWorkerThread = new Thread(WaitForNewClient);
            _clientWorkerThread.Start();

            Console.ReadKey();

            _clientWorkerThread.Abort();
            _tcpListener.Stop();
            _roomManager.Dispose();
        }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////////
        private static void WaitForNewClient()
        {
            while (true)
                try
                {
                    var tcpClient = _tcpListener.AcceptTcpClient();
                    _roomManager.AcceptClient(tcpClient);
                }
                catch (ThreadAbortException)
                {
                    //TODO: Sometimes occur when Thread disposing, maybe investigation required
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{nameof(WaitForNewClient)} something is broken: {ex.Message}");
#if DEBUG
                    throw;
#endif
                }
        }

        private static void CheckAnyOtherInstances()
        {
            var guid = Marshal.GetTypeLibGuidForAssembly(Assembly.GetExecutingAssembly()).ToString();

            bool created;
            var mutexObj = new Mutex(true, guid, out created);
            if (!created)
            {
                Console.WriteLine("Application instance already exist");
                Thread.Sleep(3000);
                Environment.Exit(0);
            }
        }
    }
}