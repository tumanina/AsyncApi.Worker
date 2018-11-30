using Newtonsoft.Json;

namespace AsyncApi.Worker.MessageBroker
{
    public class ErrorResponse
    {
        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("error_code")]
        public int ErrorCode { get; set; }
    }
}
