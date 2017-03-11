using Common.Extensions;
using GameServer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace GameServer
{
    class Program
    {
        private const String HOST = "127.0.0.1";
        private const Int32 PORT = 8888;

        private static TcpListener _tcpListener;
        private static List<Room> _rooms;


        static void Main(String[] args)
        {
            CheckAnyOtherInstances();
            WitingClients();

            Console.ReadKey();

            _tcpListener?.Stop();
        }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////////
        private static void CheckAnyOtherInstances()
        {
            bool created;
            var guid = Marshal.GetTypeLibGuidForAssembly(Assembly.GetExecutingAssembly()).ToString();
            var mutexObj = new Mutex(true, guid, out created);

            if (!created)
            {
                Console.WriteLine("Instance already exist");
                Environment.Exit(0);
            }
        }
        private static void WitingClients()
        {
            _tcpListener = new TcpListener(IPAddress.Parse(HOST), PORT);
            _tcpListener.Start();

            while (true)
            {
                using (var tcpClient = _tcpListener.AcceptTcpClient())
                {
                    try
                    {
                        var stream = tcpClient.GetStream();
                        while (true)
                        {
                            if (!stream.CanRead)
                            {
                                Thread.Sleep(10);
                                continue;
                            }

                            var clientMessage = stream.ReadString();
                            stream.WriteString($"Echo: {clientMessage}");
                        }
                    }
                    catch (IOException)
                    {

                    }
                    catch (Exception ex)
                    {
                        Console.Clear();
                        Console.WriteLine("Something is broken:");
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
        private static void AcceptNewClient(NetworkStream stream)
        {

        }
    }
}
