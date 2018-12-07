using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AsyncApi.Worker.Unit.Tests.ServiceTests
{
    [TestClass]
    public class ClientFactoryTest
    {
        private static readonly Mock<IClient> Client = new Mock<IClient>();

        [TestMethod]
        public void GetClient_Success_ShouldReturnInitializedService()
        {
            Client.Invocations.Clear();

            var configuration = new ClientConfiguration
            {
                User = "userName2",
                Password = "password2",
                Address = "host2",
                Endpoint = "endpoint2",
                Port = 8082
            };

            var config = new ClientConfiguration();

            Client.Setup(x => x.Init(It.IsAny<ClientConfiguration>()))
                .Callback<ClientConfiguration>((configurationParam) => config = configurationParam);

            var service = new ClientFactory(Client.Object);

            var result = service.GetClient(configuration);

            Client.Verify(x => x.Init(It.IsAny<ClientConfiguration>()), Times.Once);
            Assert.AreEqual(config.Address, configuration.Address);
            Assert.AreEqual(config.Port, configuration.Port);
            Assert.AreEqual(config.User, configuration.User);
            Assert.AreEqual(config.Password, configuration.Password);
            Assert.AreEqual(config.Endpoint, configuration.Endpoint);
        }

        [TestMethod]
        public void GetService_InitReturnException_ShouldReturnException()
        {
            Client.Invocations.Clear();

            var exceptionMessage = "some exception message";

            var configuration = new ClientConfiguration
            {
                User = "userName2",
                Password = "password2",
                Address = "host2",
                Endpoint = "endpoint2",
                Port = 8082
            };

            Client.Setup(x => x.Init(It.IsAny<ClientConfiguration>())).Throws(new Exception(exceptionMessage));

            var service = new ClientFactory(Client.Object);

            try
            {
                var result = service.GetClient(configuration);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, exceptionMessage);
                Client.Verify(x => x.Init(It.IsAny<ClientConfiguration>()), Times.Once);
            }
        }
    }
}
