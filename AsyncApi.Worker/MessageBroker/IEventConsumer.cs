using RabbitMQ.Client;

namespace AsyncApi.Worker.MessageBroker
{
    public interface IEventConsumer : IBasicConsumer
    {
        void Consume(IMessageProcessor processor);
    }
}
