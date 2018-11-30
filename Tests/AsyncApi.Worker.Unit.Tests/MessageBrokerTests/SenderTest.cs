using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RabbitMQ.Client;
using System;
using AsyncApi.Worker.MessageBroker;

namespace AsyncApi.Worker.Unit.Tests.MessageBrokerTests
{
    [TestClass]
    public class SenderTest
    {
        private static readonly Mock<IConnectionFactory> ConnectionFactory = new Mock<IConnectionFactory>();
        private static readonly Mock<IConnection> Connection = new Mock<IConnection>();
        private static readonly Mock<IModel> Model = new Mock<IModel>();

        [TestMethod]
        public void SendMessage_UseConnectionFactoryAndModel()
        {
            ConnectionFactory.Invocations.Clear();

            var message = "{ 'email' : 'test@test.com', 'name': 'test' }";
            var queueName = $"{Guid.NewGuid()}_queue";
            var exchangeName = "exchange";
            var exchange = string.Empty;
            var routing = string.Empty;
            var body = Encoding.UTF8.GetBytes("");

            Model.Setup(x => x.QueueBind(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null));
            Model.Setup(x => x.BasicPublish(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<IBasicProperties>(), It.IsAny<byte[]>()))
                .Callback<string, string, bool, IBasicProperties, byte[]>((exchangeParam, routingParam, mandatoryParam, propertyParam, bodyParam) =>
                {
                    exchange = exchangeParam;
                    routing = routingParam;
                    body = bodyParam;
                });

            Connection.Setup(x => x.CreateModel()).Returns(Model.Object);
            ConnectionFactory.Setup(x => x.CreateConnection()).Returns(Connection.Object);
            
            var sender = new Sender(ConnectionFactory.Object, exchangeName);

            sender.SendMessage(queueName, message);

            ConnectionFactory.Verify(x => x.CreateConnection(), Times.Once);
            Connection.Verify(x => x.CreateModel(), Times.Once);
            Model.Verify(x => x.QueueBind(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null), Times.Once);
            Model.Verify(x => x.BasicPublish(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), null, It.IsAny<byte[]>()), Times.Once);

            Assert.AreEqual(exchange, exchangeName);
            Assert.AreEqual(routing, queueName);
            Assert.AreEqual(Encoding.UTF8.GetString(body), message);
        }
    }
}
