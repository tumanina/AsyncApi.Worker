using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AsyncApi.Worker.MessageBroker
{
    public interface IConsumerFactory
    {
        EventingBasicConsumer CreateEventConsumer(IModel model);
    }
}
