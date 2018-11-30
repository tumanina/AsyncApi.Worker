using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AsyncApi.Worker.Repositories.Entities;
using AsyncApi.Worker.Repositories.Interfaces;
using AsyncApi.Worker.Enums;
using AsyncApi.Worker.Services;
using System;

namespace AsyncApi.Worker.Unit.Tests.ServiceTests
{
    [TestClass]
    public class TaskServiceTest
    {
        private static readonly Mock<ITaskRepository> TaskRepository = new Mock<ITaskRepository>();

        [TestMethod]
        public void UpdateStatus_Success_ShouldReturnTask()
        {
            TaskRepository.Invocations.Clear();

            var id = Guid.NewGuid();
            var type = 2;
            var status = TaskStatus.Completed;
            var data = "123455";
            var taskResult = "some result";

            var entity = new Task
            {
                Id = id,
                Type = (int)type,
                Status = (int)status,
                Result = taskResult,
                Data = data
            };

            TaskRepository.Setup(x => x.UpdateStatus(id, (int)status, taskResult)).Returns(entity);

            var service = new TaskService(TaskRepository.Object);

            var result = service.UpdateStatus(id, (int)status, taskResult);

            TaskRepository.Verify(x => x.UpdateStatus(id, (int)status, taskResult), Times.Once);
            Assert.AreEqual(result.Type, type);
            Assert.AreEqual(result.Status, (int)status);
            Assert.AreEqual(result.Result, taskResult);
            Assert.AreEqual(result.Data, data);
        }

        [TestMethod]
        public void UpdateStatus_ServiceReturnNull_ShouldReturnNull()
        {
            TaskRepository.Invocations.Clear();

            var id = Guid.NewGuid();
            var status = TaskStatus.Completed;
            var taskResult = "some result";

            TaskRepository.Setup(x => x.UpdateStatus(id, (int)status, taskResult)).Returns((Task)null);

            var service = new TaskService(TaskRepository.Object);

            var result = service.UpdateStatus(id, (int)status, taskResult);

            TaskRepository.Verify(x => x.UpdateStatus(id, (int)status, taskResult), Times.Once);
            Assert.AreEqual(result, null);
        }

        [TestMethod]
        public void SetError_Success_ShouldReturnTask()
        {
            TaskRepository.Invocations.Clear();

            var id = Guid.NewGuid();
            var type = 2;
            var status = TaskStatus.Failed;
            var data = "123455";
            var error = "some error";

            var entity = new Task
            {
                Id = id,
                Type = (int)type,
                Status = (int)status,
                Error = error,
                Data = data
            };

            TaskRepository.Setup(x => x.SetError(id, error)).Returns(entity);

            var service = new TaskService(TaskRepository.Object);

            var result = service.SetError(id, error);

            TaskRepository.Verify(x => x.SetError(id, error), Times.Once);
            Assert.AreEqual(result.Type, type);
            Assert.AreEqual(result.Status, (int)status);
            Assert.AreEqual(result.Error, error);
            Assert.AreEqual(result.Data, data);
        }

        [TestMethod]
        public void SetError_ServiceReturnNull_ShouldReturnNull()
        {
            TaskRepository.Invocations.Clear();

            var id = Guid.NewGuid();
            var error = "some error";

            TaskRepository.Setup(x => x.SetError(id, error)).Returns((Task)null);

            var service = new TaskService(TaskRepository.Object);

            var result = service.SetError(id, error);

            TaskRepository.Verify(x => x.SetError(id, error), Times.Once);
            Assert.AreEqual(result, null);
        }
    }
}
