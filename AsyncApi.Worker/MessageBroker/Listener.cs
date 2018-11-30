using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MultiWallet.Business.MessageBroker.Messages;
using RabbitMQ.Client;

namespace AsyncApi.Worker.MessageBroker
{
    public class Listener : IListener
    {
        private readonly IMessageProcessor _messageProcessor;
        private readonly IConnectionFactory _connectionFactory;
        private readonly IConsumerFactory _consumerFactory; 
        private readonly string _exchangeName;
        private readonly string _queueName;
        private readonly MessageType _type;
        private readonly ILogger<Listener> _logger;

        public MessageType Type => _type;

        public Listener(IConnectionFactory connectionFactory, IConsumerFactory consumerFactory, MessageType type, string queueName, 
            string exchangeName, IEnumerable<IMessageProcessor> messageProcessors, ILogger<Listener> logger)
        {
            _consumerFactory = consumerFactory;
            _type = type;
            _queueName = queueName;
            _exchangeName = exchangeName;
            _connectionFactory = connectionFactory;
            _logger = logger;

            _messageProcessor = messageProcessors.FirstOrDefault(t => t.Type == type);
        }

        public void Run()
        {
            if (_messageProcessor != null)
            {
                var connection = _connectionFactory.CreateConnection();
                var channel = connection.CreateModel();

                channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct, true);
                channel.QueueDeclare(_queueName, true, false, false, null);
                channel.QueueBind(_queueName, _exchangeName, _queueName, null);

                var consumer = _consumerFactory.CreateEventConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    _messageProcessor.Process(message);
                    channel.BasicAck(ea.DeliveryTag, false);
                };

                channel.BasicConsume(_queueName, false, consumer);
            }
            else
            {
                _logger.LogError($"Message proccesor for type '{_type.ToString()}' not found");
            }
        }

        public async Task RunAsync()
        {
            await Task.Run(() =>
            {
                Run();
            });
        }
    }
}
