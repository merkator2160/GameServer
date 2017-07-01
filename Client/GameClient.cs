using ClientManager.Models;
using Common.Extensions;
using Common.Models;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace ClientManager
{
    public class GameClient : IDisposable
    {
        private TcpClient _client;
        private Boolean _isConnectionEstablished;
        private Boolean _disposed;
        private Thread _workerThread;
        private readonly ClientConfig _config;


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


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        public void Start()
        {
            _workerThread = new Thread(DoWork);
            _workerThread.Start();
        }
        public void Stop()
        {
            _workerThread?.Abort();
            _client?.Close();
        }
        private void DoWork()
        {
            while (true)
            {
                try
                {
                    Communicate();
                }
                catch (ThreadAbortException)
                {
                    //TODO: Sometimes occur when Thread disposing, maybe investigation required
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"Client {_config.ClientId}: Server unavalible. Retrying to connect.");
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Client {_config.ClientId}: Server unavalible. Retrying to connect.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Client {_config.ClientId}: Something is broken:");
                    Console.WriteLine(ex.Message);
#if DEBUG
                    throw;
#endif
                }

                Thread.Sleep(ClientConfig.ReconnectDelay);
            }
        }
        private void Communicate()
        {
            while (true)
            {
                NetworkStream stream;
                if (_client == null || !_client.Connected)
                {
                    stream = Connect();
                    SendConnectionRequest(stream);
                    _isConnectionEstablished = true;
                }
                else
                {
                    stream = _client.GetStream();
                }

                if (_isConnectionEstablished)
                {
                    if (stream.DataAvailable)
                        ReceiveMessage(stream);
                    SendMessage(stream);
                }
                Thread.Sleep(ClientConfig.SendMessageDelay);
            }
        }
        private NetworkStream Connect()
        {
            _client = new TcpClient(_config.Host, _config.Port)
            {
                SendTimeout = ClientConfig.SendOperationsTimeout,
                ReceiveTimeout = ClientConfig.ReceiveOperationsTimeout
            };

            Console.WriteLine($"Client {_config.ClientId}: Connected");

            return _client.GetStream();
        }
        private void SendConnectionRequest(NetworkStream stream)
        {
            stream.WriteObject(new ConnectionRequest()
            {
                RoomId = _config.RoomId,
                ClientId = _config.ClientId
            });
        }
        private void SendMessage(NetworkStream stream)
        {
            stream.WriteObject(new Message()
            {
                Body = "Hi!"
            });
        }
        private void ReceiveMessage(NetworkStream stream)
        {
            var message = stream.ReadObject<Message>().Body;
            Console.WriteLine($"Client id: {_config.ClientId}, Room id: {_config.RoomId}: {message}");
        }


        // IDisposable ////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(Boolean disposing)
        {
            if (!_disposed)
            {
                ReleaseUnmanagedResources();
                if (disposing)
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
            Stop();
        }
    }
}