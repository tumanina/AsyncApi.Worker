using MultiWallet.Business.MessageBroker.Messages;
using AsyncApi.Worker.Services;

namespace AsyncApi.Worker.MessageBroker
{
    public class CreateClientMessageProcessor : MessageProcessor<CreateClientMessage>
    {
        private readonly ICustomerService _customerService;

        public CreateClientMessageProcessor(ITaskService taskService, ICustomerService customerService, ISender sender) : base(taskService, sender)
        {
            _customerService = customerService;
        }

        public override MessageType Type { get { return MessageType.CreateClient; } }

        protected override string GetResult(CreateClientMessage request)
        {
            return _customerService.CreateCustomer(request.Name, request.Email, request.Password).Result;
        }
    }
}
