using Common.Models.Metwork;

namespace Common
{
    public interface IMessageProcessor
    {
        void Handle(NetworkMessage message);
    }
}