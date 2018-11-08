using Newtonsoft.Json;
using System;

namespace MultiWallet.Business.MessageBroker.Messages
{
    public class BaseMessage
    {
        [JsonProperty("task_id")]
        public Guid TaskId { get; set; }

        [JsonProperty("callback_queue_name")]
        public string CallbackQueueName { get; set; }
    }
}
