using Common.MessageProcessing;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.Unity;
using PipelineNet.MiddlewareResolver;
using Server.Models;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace Server
{
    static class Program
    {
        static void Main(String[] args)
        {
            CheckForAnotherInstances();
            using (var container = ConfigureContainer(args))
            {
                var messenger = container.Resolve<IMessenger>();
                var consoleWriter = container.Resolve<ServerConsoleWriter>();
                var roomManager = container.Resolve<RoomManager>();
                var connectionListener = container.Resolve<ConnectionListener>();
                connectionListener.Start();

                Console.ReadKey();
            }
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
        private static IUnityContainer ConfigureContainer(String[] args)
        {
            var container = new UnityContainer();
            container.RegisterInstance(new RootConfig()
            {
                Port = ExtractCommandArgs(args),
                RoomManagerConfig = new RoomManagerConfig()
                {
                    CleanUpThreadIdle = TimeSpan.FromSeconds(1),
                    EmptyRoomLifeTime = TimeSpan.FromMinutes(1)
                }
            });
            container.RegisterType<IMessenger, Messenger>(new ContainerControlledLifetimeManager());
            container.RegisterType<ServerConsoleWriter>(new ContainerControlledLifetimeManager());
            container.RegisterType<RoomManager>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMiddlewareResolver, UnityMiddlewareResolver>();

            return container;
        }
        private static Int32 ExtractCommandArgs(String[] args)
        {
            return args.Length > 0 ? Int32.Parse(args[0]) : 8888;
        }

    }
}
