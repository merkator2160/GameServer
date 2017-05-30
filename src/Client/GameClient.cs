using ClientManager.Models;
using Common.Extensions;
using Common.Models;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ClientManager
{
    public class GameClient : IGameClient, IDisposable
    {
        private TcpClient _client;
        private CancellationTokenSource _cancelTokenSource;
        private readonly ClientConfig _config;
        private bool _disposed;


        public GameClient() : this(new ClientConfig())
        {

        }
        public GameClient(ClientConfig config)
        {
            _config = config;
        }
        ~GameClient()
        {
            Dispose(false);
        }


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public Guid Id => _config.ClientId;
        public Guid RoomId => _config.RoomId;


        // IGameClient ////////////////////////////////////////////////////////////////////////////
        public void Start()
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

                                Thread.Sleep(ClientConfig.RepeatRequestTime);
                            }
                        }
                        catch(SocketException)
                        {
                            Console.WriteLine($"Client id: {Id} - Server unavalible. Retry to connect.");
                            Thread.Sleep(ClientConfig.ReconnectToServerTimeout);
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


        // SUPPORT FUNCTIONS //////////////////////////////////////////////////////////////////////
        private NetworkStream Reconnect()
        {
            _client = new TcpClient(_config.Host, _config.Port)
            {
                SendTimeout = ClientConfig.SendReceiveOperationsTimeout,
                ReceiveTimeout = ClientConfig.SendReceiveOperationsTimeout
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


        // IDisposable ////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if(!_disposed)
            {
                ReleaseUnmanagedResources();
                if(disposing)
                    ReleaseManagedResources();

                _disposed = true;
            }
        }
        private void ReleaseUnmanagedResources()
        {

        }
        private void ReleaseManagedResources()
        {
            _client?.Close();
            _cancelTokenSource?.Dispose();
        }
    }
}