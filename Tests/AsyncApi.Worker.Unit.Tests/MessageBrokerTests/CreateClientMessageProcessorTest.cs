using System;
using AsyncApi.Worker.MessageBroker;
using AsyncApi.Worker.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MultiWallet.Business.MessageBroker.Messages;

namespace AsyncApi.Worker.Unit.Tests.MessageBrokerTests
{
    [TestClass]
    public class CreateClientMessageProcessorTest
    {
        private static readonly Mock<ITaskService> TaskService = new Mock<ITaskService>();
        private static readonly Mock<ISender> Sender = new Mock<ISender>();

        [TestMethod]
        public void Process_GetResult_ShouldSendResultAndUpdateTask()
        {
            TaskService.Invocations.Clear();
            Sender.Invocations.Clear();

            var taskId = Guid.NewGuid();

            var queueName = "144_CallbackQueue";
            var message = "{\"email\" : \"test@test.com\", \"name\": \"test\", \"task_id\":\"" + taskId + "\",\"callback_queue_name\":\"" + queueName + "\"}";
            var result = string.Empty;

            var createdAddress = string.Empty;

            TaskService.Setup(x => x.UpdateStatus(taskId, 5, It.IsAny<string>()))
                .Callback<Guid, int, string>((idParam, statusParam, messageParam) => { result = messageParam; });

            var message1 = new CreateClientMessage();
            var sendingMessage = string.Empty;

            TaskService.Setup(x => x.SetError(taskId, It.IsAny<string>()));
            Sender.Setup(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>())).Callback<string, string>((queueParam, messageParam) => { sendingMessage = messageParam; });

            var service = new CreateClientMessageProcessor(TaskService.Object, Sender.Object);

            service.Process(message);

            TaskService.Verify(x => x.UpdateStatus(taskId, 5, It.IsAny<string>()), Times.Once);
            TaskService.Verify(x => x.SetError(taskId, It.IsAny<string>()), Times.Never);
            Sender.Verify(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.IsTrue(result.Contains(sendingMessage));
        }
    }
}
