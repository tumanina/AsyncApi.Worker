using Newtonsoft.Json;

namespace MultiWallet.Business.MessageBroker.Messages
{
    public class CreateClientMessage : BaseMessage
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
