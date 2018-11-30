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
                    try
                    {
                        model.QueueBind(queueName, _exchangeName, queueName, null);

                        byte[] messageBodyBytes = Encoding.UTF8.GetBytes(message);
                        model.BasicPublish(_exchangeName, queueName, null, messageBodyBytes);
                    }
                    catch
                    {
                        //queue already delete (by timeout consumers switched off); shouldn't do anything
                    }
                }
            }
        }
    }
}
