using Microsoft.EntityFrameworkCore;

namespace AsyncApi.Worker.Repositories.DAL
{
    public class TaskDBContextFactory : ITaskDBContextFactory
    {
        private readonly DbContextOptionsBuilder<TaskDBContext> _options;

        public TaskDBContextFactory(DbContextOptionsBuilder<TaskDBContext> options)
        {
            _options = options;
        }

        public ITaskDBContext CreateDBContext()
        {
            return new TaskDBContext(_options.Options);
        }
    }
}
