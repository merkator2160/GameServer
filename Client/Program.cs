using Client.Models;
using Common;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.Unity;
using PipelineNet.MiddlewareResolver;
using System;

namespace Client
{
    static class Program
    {
        static void Main(String[] args)
        {
            using (var container = ConfigureContainer(args))
            {
                var consoleWriter = container.Resolve<ConsoleWriter>();
                var numbersGenerator = container.Resolve<NumberGenerator>();
                var client = container.Resolve<GameClient>();
                client.Start();
                numbersGenerator.Start();

                Console.ReadKey();
            }
        }
        private static IUnityContainer ConfigureContainer(String[] args)
        {
            var container = new UnityContainer();
            container.RegisterType<IMessenger, Messenger>(new ContainerControlledLifetimeManager());
            container.RegisterInstance(CreateConfigFromCommandArgs(args));
            container.RegisterType<NumberGenerator>();
            container.RegisterType<IMiddlewareResolver, UnityMiddlewareResolver>();
            container.RegisterType<IMessageProcessor, MessageProcessor>();
            container.RegisterType<GameClient>();
            container.RegisterType<ConsoleWriter>(new ContainerControlledLifetimeManager());

            return container;
        }
        private static RootConfig CreateConfigFromCommandArgs(String[] args)
        {
            var host = args.Length > 0 ? args[0] : "127.0.0.1";
            var port = args.Length > 1 ? Int32.Parse(args[1]) : 8888;
            var nickName = args.Length > 2 ? args[1] : "Default";
            var roomId = args.Length > 3 ? Guid.Parse(args[3]) : new Guid("0b23d1e1-58a0-4122-8e9b-fa65961b0df6");

            return new RootConfig()
            {
                NickName = nickName,
                RoomId = roomId,
                NetworkConfig = new NetworkConfig()
                {
                    Host = host,
                    Port = port,
                    ReconnectDelay = 3000
                },
                NumberGeneratorConfig = new NumberGeneratorConfig()
                {
                    Delay = 1000,
                    Offset = 100
                }
            };
        }
    }
}