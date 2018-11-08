using Microsoft.EntityFrameworkCore;
using AsyncApi.Worker.Repositories.Entities;

namespace AsyncApi.Worker.Repositories.DAL
{
    public interface ITaskDBContext
    {
        DbSet<Task> Task { get; set; }
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        int SaveChanges();
    }
}
