using MultiWallet.Business.MessageBroker.Messages;
using AsyncApi.Worker.Services;
using Newtonsoft.Json;
using System;

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
            var taskId = (Guid?)null;
            var callbackQueueName = string.Empty;
            try
            {
                var model = JsonConvert.DeserializeObject<T>(message);

                taskId = model.TaskId;
                callbackQueueName = model.CallbackQueueName;
                var result = GetResult(model);

                _sender.SendMessage(callbackQueueName, result);
                _taskService.UpdateStatus(model.TaskId, 5, result);
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerMessage();

                if (!string.IsNullOrEmpty(callbackQueueName))
                {
                    _sender.SendMessage(callbackQueueName, JsonConvert.SerializeObject(new ErrorResponse { ErrorCode = 500, Error = errorMessage }));
                }

                if (taskId != null)
                {
                    _taskService.SetError(taskId.Value, errorMessage);
                }
            }
        }

        protected virtual string GetResult(T request)
        {
            return string.Empty;
        }
    }
}
