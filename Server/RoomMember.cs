using Common;
using Common.Models.Enums;
using Common.Models.Metwork;
using Common.Models.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Server
{
    internal class RoomMember : IDisposable
    {
        private readonly IMessenger _roomMessenger;
        private readonly Boolean _sendEcho;
        private Boolean _disposed;
        private readonly Dictionary<MessageType, Action<Byte[]>> _messageDictionary;


        public RoomMember(Guid id, BufferedTcpClient<NetworkMessage> client, IMessenger roomMessenger, Boolean sendEcho)
        {
            Id = id;
            _roomMessenger = roomMessenger;
            _sendEcho = sendEcho;
            Client = client;
            _messageDictionary = new Dictionary<MessageType, Action<Byte[]>>()
            {
                { MessageType.Text, HandleTextMessage }
            };

            roomMessenger.Register<TextReceiveMessage>(this, RoomMessageTextReceived);
            ThreadPool.QueueUserWorkItem(MessageProcessingThread);
        }


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public BufferedTcpClient<NetworkMessage> Client { get; }
        public Guid Id { get; }


        // THREADS ////////////////////////////////////////////////////////////////////////////////
        private void MessageProcessingThread(Object state)
        {
            while (!_disposed)
            {
                try
                {
                    if (Client.ReceivedMessageQueue.IsEmpty)
                    {
                        Thread.Sleep(10);
                        continue;
                    }

                    if (Client.ReceivedMessageQueue.TryDequeue(out NetworkMessage message))
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


        // HANDLERS ///////////////////////////////////////////////////////////////////////////////
        private void HandleTextMessage(Byte[] data)
        {
            var text = Encoding.UTF8.GetString(data);
            _roomMessenger.Send(new TextReceiveMessage(text, Id));
        }
        private void RoomMessageTextReceived(TextReceiveMessage message)
        {
            if (_sendEcho || message.Sender != Id)
            {
                Client.SendMessageQueue.Enqueue(new NetworkMessage(MessageType.Text, Encoding.UTF8.GetBytes(message.Text)));
            }
        }


        // IDisposable ////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                Client?.Dispose();
            }
        }
    }
}