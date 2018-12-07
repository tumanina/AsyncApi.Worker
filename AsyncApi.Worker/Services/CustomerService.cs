using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AsyncApi.Worker.Services
{
    public class CustomerService : ICustomerService
    {
        protected IClient Client;
        protected ILogger<ICustomerService> Logger;

        public CustomerService(IClientFactory clientFactory, ClientConfiguration configuration, ILogger<ICustomerService> logger)
        {
            Logger = logger;

            if (configuration != null)
            {
                Client = clientFactory.GetClient(configuration);
            }
        }

        public async Task<string> CreateCustomer(string name, string email, string password)
        {
            if (Client == null)
            {
                var error = "Client not initialized. Check configuration in appsettings.json file.";
                Logger.LogError(error);
                throw new InternalServerErrorException(error);
            }

            return await Client.SendRequest(Enums.HttpMethodType.POST, methodParams: new { Email = email, Name = name, Password = password });
        }
    }
}
