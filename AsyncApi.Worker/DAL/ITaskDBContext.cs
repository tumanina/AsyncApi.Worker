using Microsoft.EntityFrameworkCore;
using AsyncApi.Worker.Repositories.Entities;
using System;

namespace AsyncApi.Worker.Repositories.DAL
{
    public interface ITaskDBContext : IDisposable
    {
        DbSet<Task> Task { get; set; }
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        int SaveChanges();
    }
}
