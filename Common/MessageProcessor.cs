using Common.Models.Metwork;
using PipelineNet.MiddlewareResolver;
using PipelineNet.Pipelines;

namespace Common
{
    public class MessageProcessor : IMessageProcessor
    {
        private readonly Pipeline<NetworkMessage> _networkMessageProcessingPipeline;


        public MessageProcessor(IMiddlewareResolver chainLinkResolver)
        {
            _networkMessageProcessingPipeline = new Pipeline<NetworkMessage>(chainLinkResolver);
        }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        public void Handle(NetworkMessage message)
        {
            _networkMessageProcessingPipeline.Execute(message);
        }
    }
}