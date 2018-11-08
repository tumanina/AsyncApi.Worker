using AsyncApi.Worker.Repositories.Entities;
using System;

namespace AsyncApi.Worker.Repositories.Interfaces
{
    public interface ITaskRepository
    {
        Task UpdateStatus(Guid id, int status, string result);
        Task SetError(Guid id, string error);
    }
}
