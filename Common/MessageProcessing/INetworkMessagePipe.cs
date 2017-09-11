using Common.Models.Metwork;

namespace Common.MessageProcessing
{
    public interface INetworkMessagePipe
    {
        void Handle(NetworkMessage message);
    }
}