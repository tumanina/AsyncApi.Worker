using System.Collections.Concurrent;
using System.Text;
using RabbitMQ.Client;

namespace AsyncApi.Worker.MessageBroker
{
    public class Sender : ISender
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly string _exchangeName;

        public Sender(IConnectionFactory connectionFactory, string exchangeName)
        {
            _exchangeName = exchangeName;
            _connectionFactory = connectionFactory;
        }

        public void SendMessage(string queueName, string message)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                using (var model = connection.CreateModel())
                {
                    model.ExchangeDeclare(_exchangeName, ExchangeType.Direct, true);
                    model.QueueDeclare(queueName, false, false, true, new ConcurrentDictionary<string, object>());
                    model.QueueBind(queueName, _exchangeName, queueName, null);

                    byte[] messageBodyBytes = Encoding.UTF8.GetBytes(message);
                    model.BasicPublish(_exchangeName, queueName, null, messageBodyBytes);
                }
            }
        }
    }
}
