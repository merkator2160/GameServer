using Common.Extensions;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Common
{
    public class BufferedTcpClient<T> : IDisposable
    {
        private Boolean _disposed;
        private readonly TcpClient _client;
        private readonly NetworkStream _networkStream;


        public BufferedTcpClient(TcpClient client, Boolean keepAlive)
        {
            _client = client;
            //if (keepAlive)
            //{
            //    _client.Client.SetKeepAlive(10000, 10000);
            //}
            _networkStream = _client.GetStream();

            SendMessageQueue = new ConcurrentQueue<T>();
            ReceivedMessageQueue = new ConcurrentQueue<T>();

            ThreadPool.QueueUserWorkItem(ReceivingThread);
            ThreadPool.QueueUserWorkItem(SendingThread);
        }


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public Boolean Connected => _client.Connected;
        public ConcurrentQueue<T> SendMessageQueue { get; }
        public ConcurrentQueue<T> ReceivedMessageQueue { get; }


        // THREADS ////////////////////////////////////////////////////////////////////////////////
        private void ReceivingThread(Object state)
        {
            while (!_disposed)
            {
                try
                {
                    if (!_client.Connected || !_networkStream.DataAvailable)
                    {
                        Thread.Sleep(10);
                        continue;
                    }

                    var receivedMessage = _networkStream.ReadObject<T>();
                    ReceivedMessageQueue.Enqueue(receivedMessage);
                }
                catch (Exception ex)
                {
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
                    if (!_client.Connected || SendMessageQueue.IsEmpty)
                    {
                        Thread.Sleep(10);
                        continue;
                    }

                    if (SendMessageQueue.TryDequeue(out T message))
                    {
                        _networkStream.WriteObject(message);
                    }
                }
                catch (SocketException ex) { }
                catch (IOException ex) { }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw;
                }
            }
        }


        // IDisposable ////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                _client?.Close();
            }
        }
    }
}