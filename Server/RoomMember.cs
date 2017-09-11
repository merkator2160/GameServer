using Common.MessageProcessing;
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

namespace Server
{
    internal class RoomMember : IDisposable
    {
        private Boolean _disposed;
        private readonly String _nickName;
        private readonly IMessenger _roomMessenger;
        private readonly Boolean _sendEcho;
        private readonly Dictionary<MessageType, Action<Byte[]>> _messageDictionary;
        private readonly ConnectionBuffer _buffer;


        public RoomMember(Guid sessionId, TcpClient client, String nickName, IMessenger roomMessenger, Boolean sendEcho)
        {
            SessionId = sessionId;
            _nickName = nickName;
            _roomMessenger = roomMessenger;
            _sendEcho = sendEcho;
            _buffer = new ConnectionBuffer(true);
            _buffer.SetClient(client);
            _messageDictionary = new Dictionary<MessageType, Action<Byte[]>>()
            {
                { MessageType.Text, HandleTextMessage },
                { MessageType.KeepAlive, (data) => { } }
            };

            roomMessenger.Register<TextReceiveMessage>(this, RoomMessageTextReceived);
            ThreadPool.QueueUserWorkItem(MessageProcessingThread);
        }


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public Boolean Connected => _buffer.Connected;
        public Guid SessionId { get; }


        // THREADS ////////////////////////////////////////////////////////////////////////////////
        private void MessageProcessingThread(Object state)
        {
            while (!_disposed)
            {
                try
                {
                    if (_buffer.ReceivedMessageQueue.IsEmpty)
                    {
                        Thread.Sleep(10);
                        continue;
                    }

                    if (_buffer.ReceivedMessageQueue.TryDequeue(out NetworkMessage message))
                    {
                        _messageDictionary[message.Type].Invoke(message.Data);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw;
                }
            }
        }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        public void RefreshConnection(TcpClient client)
        {
            _buffer.SetClient(client);
        }


        // HANDLERS ///////////////////////////////////////////////////////////////////////////////
        private void HandleTextMessage(Byte[] data)
        {
            var text = Encoding.UTF8.GetString(data);
            _roomMessenger.Send(new TextReceiveMessage(text, SessionId));
        }
        private void RoomMessageTextReceived(TextReceiveMessage message)
        {
            if (_sendEcho || message.Sender != SessionId)
            {
                using (var memoryStream = new MemoryStream())
                {
                    new BinaryFormatter().Serialize(memoryStream, new TextMessage()
                    {
                        Text = message.Text,
                        From = _nickName
                    });
                    _buffer.SendMessageQueue.Enqueue(new NetworkMessage()
                    {
                        Type = MessageType.Text,
                        Data = memoryStream.ToArray()
                    });
                }
            }
        }


        // IDisposable ////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                _buffer?.Dispose();
            }
        }
    }
}