using System;
using AsyncApi.Worker.Enums;
using AsyncApi.Worker.MessageBroker;
using AsyncApi.Worker.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MultiWallet.Business.MessageBroker.Messages;

namespace AsyncApi.Worker.Unit.Tests.MessageBrokerTests
{
    [TestClass]
    public class MessageProcessorTest
    {
        private static readonly Mock<ITaskService> TaskService = new Mock<ITaskService>();
        private static readonly Mock<ISender> Sender = new Mock<ISender>();
        private static readonly string Result = "result model";
        private static readonly string ErrorMessage = "error model";

        [TestMethod]
        public void Process_Success_ShouldSendResultAndUpdateTask()
        {
            TaskService.Invocations.Clear();
            Sender.Invocations.Clear();

            var taskId = Guid.NewGuid();
            var queueName = $"{taskId}_queue";
            var message = "{\"email\" : \"test@test.com\", \"name\": \"test\", \"task_id\":\"" + taskId + "\",\"callback_queue_name\":\"" + queueName + "\"}";
            var callbackQueueName = string.Empty;
            var result = string.Empty;
            var sendingMessage = "";

            TaskService.Setup(x => x.UpdateStatus(taskId, TaskStatus.Completed, Result))
                .Callback<Guid, TaskStatus, string>((idParam, statusParam, messageParam) =>
                {
                    result = messageParam;
                });

            TaskService.Setup(x => x.SetError(taskId, ErrorMessage));
            Sender.Setup(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((queueParam, messageParam) =>
                {
                    callbackQueueName = queueParam;
                    sendingMessage = messageParam;
                });

            var service = new TestMessageProcessor1(TaskService.Object, Sender.Object);

            service.Process(message);

            TaskService.Verify(x => x.UpdateStatus(taskId, TaskStatus.Completed, Result), Times.Once);
            TaskService.Verify(x => x.SetError(taskId, ErrorMessage), Times.Never);
            Sender.Verify(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual(callbackQueueName, queueName);
            Assert.AreEqual(sendingMessage, Result);
        }

        [TestMethod]
        public void Process_MessageNotSerialized_ShouldNotSendResultAndUpdateTask()
        {
            TaskService.Invocations.Clear();
            Sender.Invocations.Clear();

            var taskId = Guid.NewGuid();
            var queueName = $"{taskId}_queue";
            var message = "queueName";

            TaskService.Setup(x => x.UpdateStatus(taskId, TaskStatus.Completed, Result));

            TaskService.Setup(x => x.SetError(taskId, ErrorMessage));
            Sender.Setup(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>()));

            var service = new TestMessageProcessor1(TaskService.Object, Sender.Object);

            service.Process(message);

            TaskService.Verify(x => x.UpdateStatus(taskId, TaskStatus.Completed, Result), Times.Never);
            TaskService.Verify(x => x.SetError(taskId, ErrorMessage), Times.Never);
            Sender.Verify(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void Process_GetResultFailed_ShouldSendMessageAndUpdateTask()
        {
            TaskService.Invocations.Clear();
            Sender.Invocations.Clear();

            var taskId = Guid.NewGuid();
            var queueName = $"{taskId}_queue";
            var message = "{\"email\" : \"test@test.com\", \"name\": \"test\", \"task_id\":\"" + taskId + "\",\"callback_queue_name\":\"" + queueName + "\"}";
            var callbackQueueName = string.Empty;

            var error = "";

            TaskService.Setup(x => x.UpdateStatus(taskId, TaskStatus.Completed, Result));
            TaskService.Setup(x => x.SetError(taskId, ErrorMessage));
            Sender.Setup(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((queueParam, messageParam) =>
                {
                    callbackQueueName = queueParam;
                    error = messageParam;
                });

            var service = new TestMessageProcessor2(TaskService.Object, Sender.Object);

            service.Process(message);

            TaskService.Verify(x => x.UpdateStatus(taskId, TaskStatus.Completed, Result), Times.Never);
            TaskService.Verify(x => x.SetError(taskId, ErrorMessage), Times.Once);
            Sender.Verify(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual(callbackQueueName, queueName);
            Assert.IsTrue(error.Contains(ErrorMessage));
        }

        private class TestMessageProcessor1 : MessageProcessor<BaseMessage>
        {
            public TestMessageProcessor1(ITaskService taskService, ISender sender) : base(taskService, sender)
            {
            }

            protected override string GetResult(BaseMessage request)
            {
                return Result;
            }
        }

        private class TestMessageProcessor2 : MessageProcessor<BaseMessage>
        {
            public TestMessageProcessor2(ITaskService taskService, ISender sender) : base(taskService, sender)
            {
            }

            protected override string GetResult(BaseMessage request)
            {
                throw new Exception(ErrorMessage);
            }
        }
    }
}
