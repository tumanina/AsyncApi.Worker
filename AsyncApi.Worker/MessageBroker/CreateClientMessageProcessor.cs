using MultiWallet.Business.MessageBroker.Messages;
using AsyncApi.Worker.Services;
using Newtonsoft.Json;

namespace AsyncApi.Worker.MessageBroker
{
    public class CreateClientMessageProcessor : MessageProcessor<CreateClientMessage>
    {

        public CreateClientMessageProcessor(ITaskService taskService, ISender sender) : base(taskService, sender)
        {
        }

        public override MessageType Type { get { return MessageType.CreateClient; } }

        protected override string GetResult(CreateClientMessage request)
        {
            return JsonConvert.SerializeObject(new { Family = "test" });
        }
    }
}
