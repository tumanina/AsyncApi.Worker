using MultiWallet.Business.MessageBroker.Messages;

namespace AsyncApi.Worker.Configuration
{
    public class SenderConfiguration
    {
        public MessageType Type { get; set; }
        public ServerConfiguration Server { get; set; }
        public string QueueName { get; set; }
        public string ExchangeName { get; set; }
    }
}
