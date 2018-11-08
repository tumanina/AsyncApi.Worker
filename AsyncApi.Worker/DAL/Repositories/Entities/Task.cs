using System;

namespace AsyncApi.Worker.Repositories.Entities
{
    public class Task
    {
        public Guid Id { get; set; }
        public int Type { get; set; }
        public string Data { get; set; }
        public string Result { get; set; }
        public int Status { get; set; }
        public string Error { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
