using System.Threading.Tasks;

namespace AsyncApi.Worker.Services
{
    public interface ICustomerService
    {
        Task<string> CreateCustomer(string name, string email, string password);
    }
}
