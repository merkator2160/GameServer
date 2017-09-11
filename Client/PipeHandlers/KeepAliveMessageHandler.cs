using Common.Models.Enums;
using Common.Models.Metwork;
using PipelineNet.Middleware;
using System;

namespace Client.PipeHandlers
{
    internal class KeepAliveMessageHandler : IMiddleware<NetworkMessage>
    {
        public void Run(NetworkMessage parameter, Action<NetworkMessage> next)
        {
            if (parameter.Type == MessageType.KeepAlive)
            {
                // TODO: maybe logging
                return;
            }

            next(parameter);
        }
    }
}