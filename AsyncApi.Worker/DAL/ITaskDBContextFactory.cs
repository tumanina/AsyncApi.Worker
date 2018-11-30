namespace AsyncApi.Worker.Repositories.DAL
{
    public interface ITaskDBContextFactory
    {
        ITaskDBContext CreateDBContext();
    }
}
