using MultiWallet.Business.MessageBroker.Messages;
using System.Threading.Tasks;

namespace AsyncApi.Worker.MessageBroker
{
    public interface IListener
    {
        MessageType Type { get; }

        void Run();
        Task RunAsync();
    }
}
