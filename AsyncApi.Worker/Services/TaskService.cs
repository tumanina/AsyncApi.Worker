using AsyncApi.Worker.Repositories.Interfaces;
using AsyncApi.Worker.Services.Models;
using System;

namespace AsyncApi.Worker.Services
{
    public class TaskService: ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public Task UpdateStatus(Guid id, int status, string result)
        {
            var task = _taskRepository.UpdateStatus(id, status, result);

            return task == null ? null : new Task(task);
        }

        public Task SetError(Guid id, string error)
        {
            var task = _taskRepository.SetError(id, error);

            return task == null ? null : new Task(task);
        }
    }
}
