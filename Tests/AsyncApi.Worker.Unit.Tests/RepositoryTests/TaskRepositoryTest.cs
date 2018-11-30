using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AsyncApi.Worker.Repositories;
using AsyncApi.Worker.Repositories.DAL;
using AsyncApi.Worker.Repositories.Entities;
using System;
using AsyncApi.Worker.Enums;

namespace AsyncApi.Worker.Unit.Tests.RepositoryTests
{
    [TestClass]
    public class TaskRepositoryTest
    {
        private static readonly Mock<ITaskDBContext> TaskDBContext = new Mock<ITaskDBContext>();
        private static readonly Mock<ITaskDBContextFactory> TaskDBContextFactory = new Mock<ITaskDBContextFactory>();

        [TestMethod]
        public void UpdateTaskStatus_TaskExist_UseDbContextReturnCorrect()
        {
            TaskDBContext.Invocations.Clear();

            var id1 = Guid.NewGuid();
            var typeId1 = 1;
            var statusId1 = 1;
            var statusId2 = 2;
            var data1 = "123455";
            var taskResult = "some result";

            var data = new List<Task>
            {
                new Task { Id = id1, Type = typeId1, Status = statusId1, Data = data1, Result = taskResult }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Task>>();

            mockSet.As<IQueryable<Task>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Task>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Task>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Task>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            mockSet.Setup(m => m.Add(It.IsAny<Task>()));

            TaskDBContext.Setup(x => x.Task).Returns(mockSet.Object);
            TaskDBContext.Setup(x => x.Set<Task>()).Returns(mockSet.Object);
            TaskDBContext.Setup(x => x.SaveChanges()).Returns(1);

            var repository = new TaskRepository(TaskDBContextFactory.Object);
            TaskDBContextFactory.Setup(x => x.CreateDBContext()).Returns(TaskDBContext.Object);

            var result = repository.UpdateStatus(id1, statusId2, taskResult);

            Assert.AreEqual(result.Status, statusId2);
            Assert.AreEqual(result.Id, id1);
            TaskDBContext.Verify(x => x.Task, Times.Exactly(2));
            TaskDBContext.Verify(x => x.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void UpdateTaskStatus_TaskNotExisted_UseDbContextReturnCorrect()
        {
            TaskDBContext.Invocations.Clear();

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var typeId1 = 1;
            var statusId1 = 1;
            var statusId2 = 2;
            var data1 = "123455";
            var taskResult = "some result";

            var data = new List<Task>
            {
                new Task { Id = id1, Type = typeId1, Status = statusId1, Data = data1, Result = taskResult }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Task>>();

            mockSet.As<IQueryable<Task>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Task>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Task>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Task>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            mockSet.Setup(m => m.Add(It.IsAny<Task>()));

            TaskDBContext.Setup(x => x.Task).Returns(mockSet.Object);
            TaskDBContext.Setup(x => x.Set<Task>()).Returns(mockSet.Object);
            TaskDBContext.Setup(x => x.SaveChanges()).Returns(1);
            TaskDBContextFactory.Setup(x => x.CreateDBContext()).Returns(TaskDBContext.Object);

            var repository = new TaskRepository(TaskDBContextFactory.Object);

            var result = repository.UpdateStatus(id2, statusId2, taskResult);

            TaskDBContext.Verify(x => x.Task, Times.Exactly(1));
            TaskDBContext.Verify(x => x.SaveChanges(), Times.Never);
            Assert.AreEqual(result, null);
        }

        [TestMethod]
        public void SetError_TaskExist_UseDbContextReturnCorrect()
        {
            TaskDBContext.Invocations.Clear();

            var id1 = Guid.NewGuid();
            var typeId1 = 1;
            var statusId1 = 1;
            var data1 = "123455";
            var error = "some error";

            var data = new List<Task>
            {
                new Task { Id = id1, Type = typeId1, Status = statusId1, Data = data1, Error = error }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Task>>();

            mockSet.As<IQueryable<Task>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Task>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Task>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Task>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            mockSet.Setup(m => m.Add(It.IsAny<Task>()));

            TaskDBContext.Setup(x => x.Task).Returns(mockSet.Object);
            TaskDBContext.Setup(x => x.Set<Task>()).Returns(mockSet.Object);
            TaskDBContext.Setup(x => x.SaveChanges()).Returns(1);
            TaskDBContextFactory.Setup(x => x.CreateDBContext()).Returns(TaskDBContext.Object);

            var repository = new TaskRepository(TaskDBContextFactory.Object);

            var result = repository.SetError(id1, error);

            Assert.AreEqual(result.Status, (int)TaskStatus.Failed);
            Assert.AreEqual(result.Error, error);
            Assert.AreEqual(result.Id, id1);
            TaskDBContext.Verify(x => x.Task, Times.Exactly(2));
            TaskDBContext.Verify(x => x.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void SetError_TaskNotExisted_UseDbContextReturnCorrect()
        {
            TaskDBContext.Invocations.Clear();

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var typeId1 = 1;
            var statusId1 = 1;
            var data1 = "123455";
            var error = "some error";

            var data = new List<Task>
            {
                new Task { Id = id1, Type = typeId1, Status = statusId1, Data = data1, Error = error }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Task>>();

            mockSet.As<IQueryable<Task>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Task>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Task>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Task>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            mockSet.Setup(m => m.Add(It.IsAny<Task>()));
            TaskDBContextFactory.Setup(x => x.CreateDBContext()).Returns(TaskDBContext.Object);

            TaskDBContext.Setup(x => x.Task).Returns(mockSet.Object);
            TaskDBContext.Setup(x => x.Set<Task>()).Returns(mockSet.Object);
            TaskDBContext.Setup(x => x.SaveChanges()).Returns(1);

            var repository = new TaskRepository(TaskDBContextFactory.Object);

            var result = repository.SetError(id2, error);

            TaskDBContext.Verify(x => x.SaveChanges(), Times.Never);
            Assert.AreEqual(result, null);
        }
    }
}
