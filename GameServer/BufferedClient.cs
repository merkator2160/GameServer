using Common.Extensions;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace GameServer
{
    public class BufferedClient<T> : IDisposable
    {
        private readonly TcpClient _client;
        private readonly NetworkStream _networkStream;

        public BufferedClient(Guid id, TcpClient client)
        {
            _client = client;
            _networkStream = _client.GetStream();
            Id = id;

            SendMessageQueue = new ConcurrentQueue<T>();
            ReceivedMessageQueue = new ConcurrentQueue<T>();

            ThreadPool.QueueUserWorkItem(ReceivingThread);
            ThreadPool.QueueUserWorkItem(SendingThread);
        }


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public Guid Id { get; }
        public Boolean IsConnected => _client.Connected;
        public ConcurrentQueue<T> SendMessageQueue { get; }
        public ConcurrentQueue<T> ReceivedMessageQueue { get; }


        // THREADS ////////////////////////////////////////////////////////////////////////////////
        private void ReceivingThread(Object state)
        {
            while (true)
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
            while (true)
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
                catch (SocketException ex)
                {

                }
                catch (IOException ex)
                {

                }
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
            _networkStream?.Close();
            _client?.Close();
        }
    }
}