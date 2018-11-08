using MultiWallet.Business.MessageBroker.Messages;
using AsyncApi.Worker.Services;
using Newtonsoft.Json;

namespace AsyncApi.Worker.MessageBroker
{
    public abstract class MessageProcessor<T> : IMessageProcessor
        where T : BaseMessage
    {
        private readonly ITaskService _taskService;
        private readonly ISender _sender;

        public virtual MessageType Type { get; set; }

        public MessageProcessor(ITaskService taskService, ISender sender)
        {
            _taskService = taskService;
            _sender = sender;
        }

        public void Process(string message)
        {
            var model = JsonConvert.DeserializeObject<T>(message);

            var taskId = model.TaskId;

            var result = GetResult(model);

            _sender.SendMessage(model.CallbackQueueName, result);

            _taskService.UpdateStatus(model.TaskId, 5, result);
        }

        protected virtual string GetResult(T request)
        {
            return string.Empty;
        }
    }
}
