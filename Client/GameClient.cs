using Client.Models;
using Common;
using Common.Extensions;
using Common.Models.Enums;
using Common.Models.Metwork;
using Common.Models.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    internal class GameClient : IDisposable
    {
        private Boolean _disposed;
        private readonly IMessenger _messenger;

        private Int32 _counter;
        private BufferedTcpClient<NetworkMessage> _bufferedClient;
        private readonly ManualResetEventSlim _workingMres;
        private readonly ManualResetEventSlim _connectedMres;
        private readonly ClientConfig _config;
        private readonly Dictionary<MessageType, Action<Byte[]>> _messageDictionary;


        public GameClient(ClientConfig config, IMessenger messenger)
        {
            _config = config;
            _messenger = messenger;
            _messenger.Register<NumberGeneratedMessage>(this, SendMessage);
            _workingMres = new ManualResetEventSlim(false);
            _connectedMres = new ManualResetEventSlim(false);
            _messageDictionary = new Dictionary<MessageType, Action<Byte[]>>()
            {
                { MessageType.Text, HandleTextMessage }
            };

            ThreadPool.QueueUserWorkItem(ConnectionThread);
            ThreadPool.QueueUserWorkItem(ReadingMessagesThread);
        }


        // THREADS ////////////////////////////////////////////////////////////////////////////////
        private void ConnectionThread(Object state)
        {
            while (!_disposed)
            {
                _workingMres.Wait();

                if (_bufferedClient == null || !_bufferedClient.Connected)
                {
                    if (!TryConnect())
                    {
                        _messenger.Send(new ConsoleMessage("Server unavalible. Retrying to connect."));
                        Thread.Sleep(_config.ReconnectDelay);
                        continue;
                    }

                    _messenger.Send(new ConsoleMessage("Connected..."));
                    _connectedMres.Set();
                }

                Thread.Sleep(10);
            }
        }
        private Boolean TryConnect()
        {
            try
            {
                var tcpClient = new TcpClient(_config.Host, _config.Port);
                SendConnectionRequest(tcpClient);
                _bufferedClient = new BufferedTcpClient<NetworkMessage>(tcpClient, false);

                return true;
            }
            catch (Exception ex)
            {
                if (ex is SocketException || ex is IOException)
                    return false;

                Debug.WriteLine(ex.Message);
                throw;
            }
        }
        private void SendConnectionRequest(TcpClient client)
        {
            var stream = client.GetStream();
            stream.WriteObject(new ConnectionRequest()
            {
                RoomId = _config.RoomId,
                ClientId = _config.ClientId
            });
        }


        private void ReadingMessagesThread(Object state)
        {
            while (!_disposed)
            {
                try
                {
                    _connectedMres.Wait();
                    if (_bufferedClient.ReceivedMessageQueue.IsEmpty)
                    {
                        Thread.Sleep(10);
                        continue;
                    }

                    if (_bufferedClient.ReceivedMessageQueue.TryDequeue(out NetworkMessage message))
                    {
                        _messageDictionary[message.Type].Invoke(message.Data);
                    }
                }
                catch (Exception ex)
                {
                    if (ex is SocketException || ex is IOException)
                    {
                        _connectedMres.Reset();
                    }

                    Debug.WriteLine(ex.Message);
                    throw;
                }
            }
        }
        private void HandleTextMessage(Byte[] data)
        {
            var text = Encoding.UTF8.GetString(data);
            _messenger.Send(new ConsoleMessage($"Client id: {_config.ClientId}, Room id: {_config.RoomId}: {text}"));
        }


        // HANDLERS ///////////////////////////////////////////////////////////////////////////////
        private void SendMessage(NumberGeneratedMessage message)
        {
            if (_bufferedClient == null || !_bufferedClient.Connected)
                return;

            var data = Encoding.UTF8.GetBytes(message.Number.ToString());
            _bufferedClient.SendMessageQueue.Enqueue(new NetworkMessage(MessageType.Text, data));
        }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        public void Start()
        {
            _workingMres.Set();
        }
        public void Stop()
        {
            _workingMres.Reset();
        }


        // IDisposable ////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                _bufferedClient?.Dispose();
                _workingMres?.Dispose();
                _connectedMres?.Dispose();
            }
        }
    }
}