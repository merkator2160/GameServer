using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using ClientManager.Models;
using Common.Extensions;
using Common.Models;

namespace ClientManager
{
    public class GameClient : IDisposable
    {
        private readonly ClientConfig _config;
        private TcpClient _client;
        private bool _disposed;
        private bool _isConnectionEstablished;
        private int _sentMessagesCounter;
        private Thread _workerThread;


        public GameClient() : this(new ClientConfig())
        {
        }

        public GameClient(ClientConfig config)
        {
            _config = config;
        }


        // IDisposable ////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
            stream.WriteObject(new ConnectionRequest
            {
                RoomId = _config.RoomId,
                ClientId = _config.ClientId
            });
        }

        private void SendMessage(NetworkStream stream)
        {
            _sentMessagesCounter++;
            stream.WriteObject(new Message
            {
                From = _config.NickName,
                Body = $"Message number {_sentMessagesCounter}"
            });
        }

        private void ReceiveMessage(NetworkStream stream)
        {
            var message = stream.ReadObject<Message>();
            Console.WriteLine($"{message.From}: {message.Body}");
        }

        protected virtual void Dispose(bool disposing)
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