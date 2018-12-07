using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AsyncApi.Worker;
using AsyncApi.Worker.Enums;
using AsyncApi.Worker.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MultiWalletWorker.UnitTests.CryptoTests
{
    [TestClass]
    public class CustomerServiceTest
    {
        private static readonly Mock<IClientFactory> ClientFactory = new Mock<IClientFactory>();
        private static readonly Mock<IClient> Client = new Mock<IClient>();
        private static List<string> _logs = new List<string>();

        [TestMethod]
        public void CreateCustomer_ClientReturnsId_ShouldReturnAddress()
        {
            ClientFactory.Invocations.Clear();
            Client.Invocations.Clear();

            var customerId = Guid.NewGuid().ToString();
            var name = "test name";
            var email = "test@test.com";
            var password = "123456";

            var configuration = new ClientConfiguration
            {
                User = "userName2",
                Password = "password2",
                Address = "host2",
                Endpoint = "endpoint2",
                Port = 8082
            };
            var response = customerId;

            ClientFactory.Setup(x => x.GetClient(configuration)).Returns(Client.Object);
            Client.Setup(x => x.SendRequest(HttpMethodType.POST, It.IsAny<object>(), null))
                .Returns(Task.FromResult(response));

            var service = new CustomerService(ClientFactory.Object, configuration, new TestLogger());

            var result = service.CreateCustomer(name, email, password).Result;

            Assert.AreEqual(result, customerId);
            ClientFactory.Verify(x => x.GetClient(configuration), Times.Once);
            Client.Verify(x => x.SendRequest(HttpMethodType.POST, It.IsAny<object>(), null), Times.Once);
        }

        [TestMethod]
        public void CreateCustomer_ClientReturnsError_ShouldThrowExceptionWithError()
        {
            ClientFactory.Invocations.Clear();
            Client.Invocations.Clear();

            var configuration = new ClientConfiguration
            {
                User = "userName2",
                Password = "password2",
                Address = "host2",
                Endpoint = "endpoint2",
                Port = 8082
            };

            var name = "test name";
            var email = "test@test.com";
            var password = "123456";
            var exceptionMessage = "test exception message";

            ClientFactory.Setup(x => x.GetClient(configuration)).Returns(Client.Object);
            Client.Setup(x => x.SendRequest(HttpMethodType.POST, It.IsAny<object>(), null))
                .Throws(new Exception(exceptionMessage));

            var service = new CustomerService(ClientFactory.Object, configuration, new TestLogger());

            try
            {
                var result = service.CreateCustomer(name, email, password).Result;
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.InnerException.Message, exceptionMessage);
                ClientFactory.Verify(x => x.GetClient(configuration), Times.Once);
                Client.Verify(x => x.SendRequest(HttpMethodType.POST, It.IsAny<object>(), null), Times.Once);
            }
        }

        [TestMethod]
        public void CreateCustomer_ClientNotCreated_ShouldThrowException()
        {
            ClientFactory.Invocations.Clear();
            Client.Invocations.Clear();

            var customerId = Guid.NewGuid().ToString();
            var name = "test name";
            var email = "test@test.com";
            var password = "123456";

            var configuration = new ClientConfiguration
            {
                User = "userName2",
                Password = "password2",
                Address = "host2",
                Endpoint = "endpoint2",
                Port = 8082
            };

            var response = "{\"error\":null,\"result\":\"" + customerId + "\"}";

            ClientFactory.Setup(x => x.GetClient(configuration)).Returns((Client)null);
            Client.Setup(x => x.InvokeMethodParams(It.IsAny<string>(), It.IsAny<object[]>()))
                .Returns(Task.FromResult(JsonConvert.DeserializeObject<JToken>(response)));
            Client.Setup(x => x.InvokeMethodObject(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(JsonConvert.DeserializeObject<JToken>(response)));

            var service = new CustomerService(ClientFactory.Object, configuration, new TestLogger());

            try
            {
                var result = service.CreateCustomer(name, email, password).Result;
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.InnerException.Message, "Client not initialized. Check configuration in appsettings.json file.");
                ClientFactory.Verify(x => x.GetClient(configuration), Times.Once);
                Client.Verify(x => x.InvokeMethodParams(It.IsAny<string>(), It.IsAny<object[]>()), Times.Never);
                Client.Verify(x => x.InvokeMethodObject(It.IsAny<string>(), It.IsAny<object>()), Times.Never);
            }
        }
        
        public class TestLogger : ILogger<ICustomerService>, IDisposable
        {
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                _logs.Add(state.ToString());
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return this;
            }

            public void Dispose()
            {
            }
        }
    }
}
