using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AsyncApi.Worker.MessageBroker
{
    public class ConsumerFactory : IConsumerFactory
    {
        public EventingBasicConsumer CreateEventConsumer(IModel model)
        {
            return new EventingBasicConsumer(model);
        }
    }
}
