using Common.Models.Enums;
using Common.Models.Metwork;
using Common.Models.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PipelineNet.Middleware;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Client.PipeHandlers
{
    internal class TextMessageHandler : IMiddleware<NetworkMessage>
    {
        private readonly IMessenger _messenger;


        public TextMessageHandler(IMessenger messenger)
        {
            _messenger = messenger;
        }


        // IMiddleware<NetworkMessage> ////////////////////////////////////////////////////////////
        public void Run(NetworkMessage parameter, Action<NetworkMessage> next)
        {
            if (parameter.Type == MessageType.Text)
            {
                using (var memoryStream = new MemoryStream(parameter.Data))
                {
                    var textMessage = new BinaryFormatter().Deserialize(memoryStream) as TextMessage;
                    _messenger.Send(new ConsoleMessage($"{textMessage.From}: {textMessage.Text}"));
                    return;
                }
            }

            next(parameter);
        }
    }
}