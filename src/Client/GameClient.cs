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
        private CancellationTokenSource _messageSenderCancelTokenSource;
        private CancellationTokenSource _messageReaderCancelTokenSource;
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
            while(true)
            {
                try
                {
                    if(_client == null || !_client.Connected)
                    {
                        var stream = Reconnect();
                        SendConnectionRequest(stream);

                        _messageSenderCancelTokenSource = new CancellationTokenSource();
                        RunMessageSender(stream, _messageSenderCancelTokenSource.Token);

                        _messageReaderCancelTokenSource = new CancellationTokenSource();
                        RunMessageReader(stream, _messageReaderCancelTokenSource.Token);
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
                catch(SocketException)
                {
                    Console.WriteLine($"Client id: {Id} - Server unavalible. Retry to connect.");
                    Thread.Sleep(ClientConfig.ReconnectToServerTimeout);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Client id: {Id} - Something is broken:");
                    Console.WriteLine(ex.Message);
#if DEBUG
                    throw;
#endif
                }
            }
        }
        public void Stop()
        {
            StopAndDisposeAll();
        }


        // SUPPORT FUNCTIONS //////////////////////////////////////////////////////////////////////
        private NetworkStream Reconnect()
        {
            Console.WriteLine("Reconnect");
            _client = new TcpClient(_config.Host, _config.Port)
            {
                SendTimeout = ClientConfig.SendReceiveOperationsTimeout,
                ReceiveTimeout = ClientConfig.SendReceiveOperationsTimeout
            };
            return _client.GetStream();
        }
        private void RunMessageReader(NetworkStream stream, CancellationToken token)
        {
            Task.Factory.StartNew(() =>
            {
                while(true)
                {
                    if(token.IsCancellationRequested)
                        return;

                    if(stream.DataAvailable)
                        ReadMessage(stream);

                    Thread.Sleep(ClientConfig.ReceiveMessageDelay);
                }
            }, token);
        }
        private void RunMessageSender(NetworkStream stream, CancellationToken token)
        {
            Task.Factory.StartNew(() =>
            {
                while(true)
                {
                    if(token.IsCancellationRequested)
                        return;

                    SendMessage(stream);

                    Thread.Sleep(ClientConfig.SendMessageDelay);
                }
            }, token);
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
        private void ReadMessage(NetworkStream stream)
        {
            var echo = stream.ReadString();
            Console.WriteLine($"Client id: {Id}, Room id: {RoomId}, Responce: {echo}");
        }
        private void StopAndDisposeAll()
        {
            _messageSenderCancelTokenSource?.Cancel();
            _messageSenderCancelTokenSource?.Dispose();

            _messageReaderCancelTokenSource?.Cancel();
            _messageReaderCancelTokenSource?.Dispose();

            _client?.Close();
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
            // We didn't have its yet.
        }
        private void ReleaseManagedResources()
        {
            StopAndDisposeAll();
        }
    }
}