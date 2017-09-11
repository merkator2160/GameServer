using Common.Extensions;
using Common.Models.Enums;
using Common.Models.Metwork;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Common.MessageProcessing
{
    public class ConnectionBuffer : IDisposable
    {
        private Boolean _disposed;
        private TcpClient _client;
        private NetworkStream _networkStream;
        private readonly ManualResetEventSlim _workingMres;


        public ConnectionBuffer(Boolean keepALive)
        {
            _workingMres = new ManualResetEventSlim(false);
            SendMessageQueue = new ConcurrentQueue<NetworkMessage>();
            ReceivedMessageQueue = new ConcurrentQueue<NetworkMessage>();

            ThreadPool.QueueUserWorkItem(ReceivingThread);
            ThreadPool.QueueUserWorkItem(SendingThread);
            if (keepALive)
            {
                ThreadPool.QueueUserWorkItem(KeepAliveThread);
            }
        }


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public Boolean Connected
        {
            get
            {
                if (_client == null)
                    return false;
                return _client.Connected;
            }
        }
        public ConcurrentQueue<NetworkMessage> SendMessageQueue { get; }
        public ConcurrentQueue<NetworkMessage> ReceivedMessageQueue { get; }


        // THREADS ////////////////////////////////////////////////////////////////////////////////
        private void ReceivingThread(Object state)
        {
            while (!_disposed)
            {
                try
                {
                    _workingMres.Wait();
                    if (!_client.Connected || !_networkStream.DataAvailable)
                    {
                        Thread.Sleep(10);
                        continue;
                    }

                    var receivedMessage = _networkStream.ReadObject<NetworkMessage>();
                    ReceivedMessageQueue.Enqueue(receivedMessage);
                }
                catch (Exception ex)
                {
                    if (ex is SocketException || ex is IOException)
                        continue;

                    Debug.WriteLine(ex.Message);
                    throw;
                }
            }
        }
        private void SendingThread(Object state)
        {
            while (!_disposed)
            {
                try
                {
                    _workingMres.Wait();
                    if (_client == null || !_client.Connected || SendMessageQueue.IsEmpty)
                    {
                        Thread.Sleep(10);
                        continue;
                    }

                    if (SendMessageQueue.TryDequeue(out NetworkMessage message))
                    {
                        _networkStream.WriteObject(message);
                    }
                }
                catch (Exception ex)
                {
                    if (ex is SocketException || ex is IOException)
                        continue;

                    Debug.WriteLine(ex.Message);
                    throw;
                }
            }
        }
        private void KeepAliveThread(Object state)
        {
            while (!_disposed)
            {
                try
                {
                    _workingMres.Wait();
                    if (!_client.Connected)
                    {
                        Thread.Sleep(10);
                        continue;
                    }

                    SendMessageQueue.Enqueue(new NetworkMessage()
                    {
                        Type = MessageType.KeepAlive
                    });
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    if (ex is SocketException || ex is IOException)
                        continue;

                    Debug.WriteLine(ex.Message);
                    throw;
                }
            }
        }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        public void SetClient(TcpClient client)
        {
            _workingMres.Reset();
            Thread.Sleep(20);

            _client?.Close();
            _client = client;
            _networkStream = client.GetStream();

            _workingMres.Set();
        }


        // IDisposable ////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                _client?.Close();
                _workingMres?.Dispose();
            }
        }
    }
}