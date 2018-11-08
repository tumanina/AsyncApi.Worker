using AsyncApi.Worker.Services.Models;
using System;

namespace AsyncApi.Worker.Services
{
    public interface ITaskService
    {
        Task UpdateStatus(Guid id, int status, string result);
        Task SetError(Guid id, string error);
    }
}
