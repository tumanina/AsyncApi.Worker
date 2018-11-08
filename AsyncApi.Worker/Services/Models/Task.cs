using System;

namespace AsyncApi.Worker.Services.Models
{
    public class Task
    {

        public Task()
        {
        }

        public Task(Repositories.Entities.Task entity)
        {
            Id = entity.Id;
            Type = entity.Type;
            Data = entity.Data;
            Result = entity.Result;
            Status = entity.Status;
            Error = entity.Error;
        }

        public Guid Id { get; set; }
        public int Type { get; set; }
        public string Data { get; set; }
        public string Result { get; set; }
        public int Status { get; set; }
        public string Error { get; set; }
    }
}
