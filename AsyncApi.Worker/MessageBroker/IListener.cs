using System.Threading.Tasks;

namespace AsyncApi.Worker.MessageBroker
{
    public interface IListener
    {
        void Run();
        Task RunAsync();
    }
}
