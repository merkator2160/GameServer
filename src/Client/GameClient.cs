using Common.Extensions;
using Common.Models;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ClientManager
{
    public class GameClient
    {
        private const String DefaultHost = "127.0.0.1";
        private const Int32 DefaultPort = 8888;
        private const Int32 RepeatRequestTime = 100;
        private const Int32 ReconnectToServerTimeout = 3000;
        private const Int32 SendReceiveOperationsTimeout = 3000;

        private TcpClient _client;
        private CancellationTokenSource _cancelTokenSource;

        private readonly String _host;
        private readonly Int32 _port;


        public GameClient(Guid clientId, Guid roomId)
        {
            _host = DefaultHost;
            _port = DefaultPort;
            Id = clientId;
            RoomId = roomId;
        }
        public GameClient(String host, Int32 port, Guid clientId, Guid roomId)
        {
            _host = host;
            _port = port;
            Id = clientId;
            RoomId = roomId;
        }


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public Guid Id { get; }
        public Guid RoomId { get; }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        public void Run()
        {
            using(_cancelTokenSource = new CancellationTokenSource())
            {
                var token = _cancelTokenSource.Token;
                Task.Factory.StartNew(() =>
                {
                    while(true)
                    {
                        try
                        {
                            var stream = Reconnect();
                            SendConnectionRequest(stream);
                            while(true)
                            {
                                if(token.IsCancellationRequested)
                                    return;

                                SendMessage(stream);
                                ReceiveEcho(stream);

                                Thread.Sleep(RepeatRequestTime);
                            }
                        }
                        catch(SocketException)
                        {
                            Console.WriteLine($"Client id: {Id} - Server unavalible. Retry to connect.");
                            Thread.Sleep(ReconnectToServerTimeout);
                        }
                        catch(Exception ex)
                        {
                            Console.Clear();
                            Console.WriteLine($"Client id: {Id} - Something is broken:");
                            Console.WriteLine(ex.Message);
                        }
                        finally
                        {
                            _client.Close();
                        }
                    }
                }, token);
            }
        }
        public void Stop()
        {
            _cancelTokenSource?.Cancel();
        }
        private NetworkStream Reconnect()
        {
            _client = new TcpClient(_host, _port)
            {
                SendTimeout = SendReceiveOperationsTimeout,
                ReceiveTimeout = SendReceiveOperationsTimeout
            };
            return _client.GetStream();
        }
        private void SendConnectionRequest(NetworkStream stream)
        {
            stream.Write(new ConnectionRequest()
            {
                RoomId = Guid.NewGuid(),
                ClientId = Guid.NewGuid()
            });
        }
        private void SendMessage(NetworkStream stream)
        {
            stream.Write(new Message()
            {
                Body = "Hi!"
            });
        }
        private void ReceiveEcho(NetworkStream stream)
        {
            Console.WriteLine($"Client id: {Id}, Room id: {RoomId}, Responce: {stream.ReadString()}");
        }
    }
}