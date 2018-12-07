using System.Collections.Generic;
using System.Threading.Tasks;
using AsyncApi.Worker.Enums;
using Newtonsoft.Json.Linq;

namespace AsyncApi.Worker
{
    public interface IClient
    {
        string Version { get; }
        void Init(ClientConfiguration configuration);
        Task<JToken> InvokeMethodObject(string method, object methodParams);
        Task<JToken> InvokeMethodParams(string method, params object[] methodParams);
        Task<string> SendRequest(HttpMethodType methodType, object methodParams = null, Dictionary<string, string> headers = null);
    }
}
