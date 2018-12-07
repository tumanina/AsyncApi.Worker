using AsyncApi.Worker.Enums;
using AsyncApi.Worker.Services.Models;
using System;

namespace AsyncApi.Worker.Services
{
    public interface ITaskService
    {
        Task UpdateStatus(Guid id, TaskStatus status, string result);
        Task SetError(Guid id, string error);
    }
}
