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
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    internal class GameClient : IDisposable
    {
        private Guid _sessionId = Guid.Empty;
        private Boolean _disposed;

        private readonly IMessenger _messenger;
        private readonly ConnectionBuffer _bufferedClient;
        private readonly ManualResetEventSlim _workingMres;
        private readonly ManualResetEventSlim _connectedMres;
        private readonly RootConfig _config;
        private readonly Dictionary<MessageType, Action<Byte[]>> _messageDictionary;


        public GameClient(RootConfig config, IMessenger messenger)
        {
            _config = config;
            _messenger = messenger;
            _bufferedClient = new ConnectionBuffer(false);
            _messenger.Register<NumberGeneratedMessage>(this, OnNumberGenerated);
            _workingMres = new ManualResetEventSlim(false);
            _connectedMres = new ManualResetEventSlim(false);
            _messageDictionary = new Dictionary<MessageType, Action<Byte[]>>()
            {
                { MessageType.Text, HandleTextMessage },
                { MessageType.KeepAlive, (data) => { } }
            };

            ThreadPool.QueueUserWorkItem(ConnectionControlThread);
            ThreadPool.QueueUserWorkItem(ReadingMessagesThread);
        }


        // THREADS ////////////////////////////////////////////////////////////////////////////////
        private async void ConnectionControlThread(Object state)
        {
            while (!_disposed)
            {
                _workingMres.Wait();

                if (_bufferedClient == null || !_bufferedClient.Connected)
                {
                    if (!await TryConnectAsync())
                    {
                        _messenger.Send(new ConsoleMessage("Server unavalible. Retrying to connect."));
                        Thread.Sleep(_config.NetworkConfig.ReconnectDelay);
                        continue;
                    }

                    _messenger.Send(new ConsoleMessage("Connected..."));
                    _connectedMres.Set();
                }

                Thread.Sleep(10);
            }
        }
        private async Task<Boolean> TryConnectAsync()
        {
            try
            {
                var tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(_config.NetworkConfig.Host, _config.NetworkConfig.Port);

                var stream = tcpClient.GetStream();
                await stream.WriteObjectAsync(new ConnectionRequest()
                {
                    RoomId = _config.RoomId,
                    SessionId = _sessionId,
                    NickName = _config.NickName
                });
                var response = await stream.ReadObjectAsync<ConnectionResponse>();
                if (response.SessionId == Guid.Empty)
                    return false;

                _sessionId = response.SessionId;
                _bufferedClient.SetClient(tcpClient);
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
            using (var memoryStream = new MemoryStream(data))
            {
                var textMessage = new BinaryFormatter().Deserialize(memoryStream) as TextMessage;
                _messenger.Send(new ConsoleMessage($"{textMessage.From}: {textMessage.Text}"));
            }
        }


        // HANDLERS ///////////////////////////////////////////////////////////////////////////////
        private void OnNumberGenerated(NumberGeneratedMessage message)
        {
            _bufferedClient.SendMessageQueue.Enqueue(new NetworkMessage()
            {
                Type = MessageType.Text,
                Data = Encoding.UTF8.GetBytes(message.Number.ToString())
            });
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