using MultiWallet.Business.MessageBroker.Messages;

namespace AsyncApi.Worker.MessageBroker
{
    public interface IMessageProcessor
    {
        MessageType Type { get; set; }

        void Process(string message);
    }
}
