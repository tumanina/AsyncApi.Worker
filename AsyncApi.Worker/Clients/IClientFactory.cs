namespace AsyncApi.Worker
{
    public interface IClientFactory
    {
        IClient GetClient(ClientConfiguration config);
    }
}
