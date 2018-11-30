using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RabbitMQ.Client;
using MultiWallet.Business.MessageBroker.Messages;
using AsyncApi.Worker.MessageBroker;
using System;
using RabbitMQ.Client.Events;

namespace AsyncApi.Worker.Unit.Tests.MessageBrokerTests
{
    [TestClass]
    public class ListenerTest
    {
        private static readonly Mock<IConnectionFactory> ConnectionFactory = new Mock<IConnectionFactory>();
        private static readonly Mock<IConnection> Connection = new Mock<IConnection>();
        private static readonly Mock<IModel> Model = new Mock<IModel>();
        private static readonly Mock<IConsumerFactory> ConsumerFactory = new Mock<IConsumerFactory>();
        private static readonly Mock<IMessageProcessor> MessageProcessor1 = new Mock<IMessageProcessor>();
        private static readonly Mock<ILogger<Listener>> Logger = new Mock<ILogger<Listener>>();

        [TestMethod]
        public void Run_ProcessorExists_ShouldRunAndListenMessages()
        {
            ResetCalls();

            var message = "{ 'email' : 'test@test.com', 'name': 'test' }";
            var queueName1 = string.Empty;
            bool? durable = null;
            bool? exclusive = null;
            bool? autoDelete = null;
            bool? exchangeDurable = null;
            bool? exchangeAutoDelete = null;
            var exchange = string.Empty;
            var body = Encoding.UTF8.GetBytes(message);

            var type = MessageType.CreateClient;
            var queueName = $"{Guid.NewGuid()}_queue";
            var exchangeName = "exchange name";
            var processedMessage = string.Empty;

            var consumer = new EventingBasicConsumer(Model.Object);
            ConsumerFactory.Setup(x => x.CreateEventConsumer(Model.Object)).Returns(consumer);

            Model.Setup(x => x.QueueDeclare(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IDictionary<string, object>>()))
                .Callback<string, bool, bool, bool, IDictionary<string, object>>((queueParam, durableParam, exclusiveParam, autoDeleteParam, param) =>
                {
                    queueName1 = queueParam;
                    durable = durableParam;
                    exclusive = exclusiveParam;
                    autoDelete = autoDeleteParam;
                });

            Model.Setup(x => x.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false, null))
                .Callback<string, string, bool, bool, IDictionary<string, object>>((exchangeParam, directParam, durableParam, autoDeleteParam, param) =>
                {
                    exchange = exchangeParam;
                    exchangeDurable = durableParam;
                    exchangeAutoDelete = autoDeleteParam;
                });
            Model.Setup(x => x.QueueBind(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null));

            Connection.Setup(x => x.CreateModel()).Returns(Model.Object);
            ConnectionFactory.Setup(x => x.CreateConnection()).Returns(Connection.Object);

            MessageProcessor1.Setup(x => x.Type).Returns(MessageType.CreateClient);
            MessageProcessor1.Setup(x => x.Process(It.IsAny<string>())).Callback<string>((messageParam) => processedMessage = messageParam);

            var listener = new Listener(ConnectionFactory.Object, ConsumerFactory.Object, type, queueName, exchangeName, new List<IMessageProcessor> { MessageProcessor1.Object }, Logger.Object);

            listener.Run();

            consumer.HandleBasicDeliver("consumer tag", 1, false, exchangeName, exchangeName, null, body);

            Assert.AreEqual(processedMessage, message);
            Assert.AreEqual(durable, true);
            Assert.AreEqual(exclusive, false);
            Assert.AreEqual(autoDelete, false);
            Assert.AreEqual(exchange, exchangeName);
            Assert.AreEqual(exchangeDurable, true);
            Assert.AreEqual(exchangeAutoDelete, false);
            Assert.AreEqual(Encoding.UTF8.GetString(body), message);

            ConnectionFactory.Verify(x => x.CreateConnection(), Times.Once);
            Connection.Verify(x => x.CreateModel(), Times.Once);
            Model.Verify(x => x.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false, null), Times.Once);
            Model.Verify(x => x.QueueDeclare(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(),It.IsAny<IDictionary<string, object>>()), Times.Once);
            Model.Verify(x => x.QueueBind(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null), Times.Once);
            Model.Verify(x => x.BasicAck(It.IsAny<ulong>(), It.IsAny<bool>()), Times.Once);
            MessageProcessor1.Verify(x => x.Process(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void Run_ProcessorNotExisted_BreakAndDontReadMessagesNoException()
        {
            ResetCalls();

            var message = "{ 'email' : 'test@test.com', 'name': 'test' }";
            var queueName1 = string.Empty;
            bool? durable = null;
            bool? exclusive = null;
            bool? autoDelete = null;
            bool? exchangeDurable = null;
            bool? exchangeAutoDelete = null;
            var exchange = string.Empty;
            var body = Encoding.UTF8.GetBytes(message);

            var type = MessageType.CreateClient;
            var queueName = $"{Guid.NewGuid()}_queue";
            var exchangeName = "exchange name";
            var processedMessage = string.Empty;

            Model.Setup(x => x.QueueDeclare(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IDictionary<string, object>>()))
                .Callback<string, bool, bool, bool, IDictionary<string, object>>((queueParam, durableParam, exclusiveParam, autoDeleteParam, param) =>
                {
                    queueName1 = queueParam;
                    durable = durableParam;
                    exclusive = exclusiveParam;
                    autoDelete = autoDeleteParam;
                });

            Model.Setup(x => x.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false, null))
                .Callback<string, string, bool, bool, IDictionary<string, object>>((exchangeParam, directParam, durableParam, autoDeleteParam, param) =>
                {
                    exchange = exchangeParam;
                    exchangeDurable = durableParam;
                    exchangeAutoDelete = autoDeleteParam;
                });
            Model.Setup(x => x.QueueBind(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null));

            Connection.Setup(x => x.CreateModel()).Returns(Model.Object);
            ConnectionFactory.Setup(x => x.CreateConnection()).Returns(Connection.Object);

            MessageProcessor1.Setup(x => x.Type).Returns(MessageType.CreateClient);
            MessageProcessor1.Setup(x => x.Process(It.IsAny<string>())).Callback<string>((messageParam) => processedMessage = messageParam);

            var listener = new Listener(ConnectionFactory.Object, ConsumerFactory.Object, type, queueName, exchangeName, new List<IMessageProcessor> { MessageProcessor1.Object }, Logger.Object);

            ConnectionFactory.Verify(x => x.CreateConnection(), Times.Never);
            Connection.Verify(x => x.CreateModel(), Times.Never);
            Model.Verify(x => x.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false, null), Times.Never);
            Model.Verify(x => x.QueueDeclare(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IDictionary<string, object>>()), Times.Never);
            Model.Verify(x => x.QueueBind(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null), Times.Never);
            Model.Verify(x => x.BasicAck(It.IsAny<ulong>(), It.IsAny<bool>()), Times.Never);
            MessageProcessor1.Verify(x => x.Process(It.IsAny<string>()), Times.Never);
        }

        private void ResetCalls()
        {
            ConnectionFactory.Invocations.Clear();
            Connection.Invocations.Clear();
            Model.Invocations.Clear();
            MessageProcessor1.Invocations.Clear();
        }
    }
}
