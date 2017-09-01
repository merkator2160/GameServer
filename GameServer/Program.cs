using Common.Models;
using GameServer.Models;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace GameServer
{
    static class Program
    {
        private static Int32 _port;

        private static ConnectionListener _connectionListener;
        private static RoomManager _roomManager;


        static void Main(String[] args)
        {
            CheckForAnotherInstances();
            ExtractCommandArgs(args);

            _roomManager = new RoomManager(new RoomManagerConfig()
            {
                CleanUpThreadIdle = TimeSpan.FromSeconds(1),
                EmptyRoomLifeTime = TimeSpan.FromMinutes(1)
            });
            _roomManager.UserNotificationAvaliable += RoomManagerOnUserNotificationAvaliable;
            _connectionListener = new ConnectionListener(_port);
            _connectionListener.NewClientAvaliable += ConnectionListenerOnNewClientAvaliable;


            Console.ReadKey();

            _connectionListener?.Dispose();
            _roomManager?.Dispose();
        }

        private static void RoomManagerOnUserNotificationAvaliable(Object sender, UserNotificationAvaliableEventArgs userNotificationAvaliableEventArgs)
        {
            Console.Clear();
            Console.WriteLine(userNotificationAvaliableEventArgs.Message);
        }


        // EVENTS /////////////////////////////////////////////////////////////////////////////////
        private static void ConnectionListenerOnNewClientAvaliable(Object sender, NewClientAvaliableEventArgs newClientAvaliableEventArgs)
        {
            _roomManager.AcceptClient(newClientAvaliableEventArgs.Client);
        }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        private static void CheckForAnotherInstances()
        {
            var guid = Marshal.GetTypeLibGuidForAssembly(Assembly.GetExecutingAssembly()).ToString();

            Boolean created;
            var mutexObj = new Mutex(true, guid, out created);
            if (!created)
            {
                Console.WriteLine("Application instance already exist");
                Thread.Sleep(3000);
                Environment.Exit(0);
            }
        }
        private static void ExtractCommandArgs(String[] args)
        {
            _port = args.Length > 0 ? Int32.Parse(args[0]) : 8888;
        }
    }
}
