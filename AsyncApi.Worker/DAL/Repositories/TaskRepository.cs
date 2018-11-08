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
        private readonly ITaskDBContext _context;

        public TaskRepository(ITaskDBContext context)
        {
            _context = context;
        }

        public Task UpdateStatus(Guid id, int status, string result)
        {
            var task = _context.Task.SingleOrDefault(b => b.Id == id);
            if (task != null)
            {
                task.Status = status;
                task.UpdatedDate = DateTime.UtcNow;
                task.Result = result;
                _context.SaveChanges();
            }
            else
            {
                return null;
            }

            return _context.Task.AsNoTracking().FirstOrDefault(t => t.Id == id);
        }

        public Task SetError(Guid id, string error)
        {
            var task = _context.Task.SingleOrDefault(b => b.Id == id);
            if (task != null)
            {
                task.Status = (int)TaskStatus.Failed;
                task.UpdatedDate = DateTime.UtcNow;
                task.Error = error;
                _context.SaveChanges();
            }
            else
            {
                return null;
            }

            return _context.Task.AsNoTracking().FirstOrDefault(t => t.Id == id);
        }
    }
}
