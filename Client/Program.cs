using Client.Models;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.Unity;
using System;

namespace Client
{
    static class Program
    {
        static void Main(String[] args)
        {
            var container = ConfigureContainer(args);
            var consoleWriter = container.Resolve<ConsoleWriter>();
            var numbersGenerator = container.Resolve<NumberGenerator>();
            var client = container.Resolve<GameClient>();
            client.Start();
            numbersGenerator.Start();

            Console.ReadKey();

            container.Dispose();
        }
        private static IUnityContainer ConfigureContainer(String[] args)
        {
            var container = new UnityContainer();
            container.RegisterType<IMessenger, Messenger>(new ContainerControlledLifetimeManager());
            container.RegisterInstance(CreateConfigFromCommandArgs(args));
            container.RegisterInstance(new NumberGenerator(1000, 100, container.Resolve<IMessenger>()));
            container.RegisterType<GameClient>();
            container.RegisterType<ConsoleWriter>(new ContainerControlledLifetimeManager());

            return container;
        }
        private static Config CreateConfigFromCommandArgs(String[] args)
        {
            var host = args.Length > 0 ? args[0] : "127.0.0.1";
            var port = args.Length > 1 ? Int32.Parse(args[1]) : 8888;
            var clientId = args.Length > 2 ? Guid.Parse(args[2]) : Guid.NewGuid();
            var roomId = args.Length > 3 ? Guid.Parse(args[3]) : Guid.NewGuid();

            return new Config(host, port, clientId, roomId);
        }
    }
}