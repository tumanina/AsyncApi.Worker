namespace AsyncApi.Worker
{
    public class ClientFactory : IClientFactory
    {
        private readonly IClient _client;

        public ClientFactory(IClient client)
        {
            _client = client;
        }

        public IClient GetClient(ClientConfiguration config)
        {
            _client.Init(config);
            return _client;
        }
    }
}
