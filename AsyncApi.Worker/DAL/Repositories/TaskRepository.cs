using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AsyncApi.Worker.Repositories.DAL;
using AsyncApi.Worker.Repositories.Entities;
using AsyncApi.Worker.Repositories.Interfaces;
using AsyncApi.Worker.Enums;

namespace AsyncApi.Worker.Repositories
{
    public class TaskRepository: ITaskRepository
    {
        private readonly ITaskDBContextFactory _factory;

        public TaskRepository(ITaskDBContextFactory factory)
        {
            _factory = factory;
        }

        public Task UpdateStatus(Guid id, int status, string result)
        {
            using (var context = _factory.CreateDBContext())
            {
                var task = context.Task.SingleOrDefault(b => b.Id == id);
                if (task != null)
                {
                    task.Status = status;
                    task.UpdatedDate = DateTime.UtcNow;
                    task.Result = result;
                    context.SaveChanges();
                }
                else
                {
                    return null;
                }

                return context.Task.AsNoTracking().FirstOrDefault(t => t.Id == id);
            }
        }

        public Task SetError(Guid id, string error)
        {
            using (var context = _factory.CreateDBContext())
            {
                var task = context.Task.SingleOrDefault(b => b.Id == id);
                if (task != null)
                {
                    task.Status = (int)TaskStatus.Failed;
                    task.UpdatedDate = DateTime.UtcNow;
                    task.Error = error;
                    context.SaveChanges();
                }
                else
                {
                    return null;
                }

                return context.Task.AsNoTracking().FirstOrDefault(t => t.Id == id);
            }
        }
    }
}
