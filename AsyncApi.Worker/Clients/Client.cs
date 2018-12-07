using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AsyncApi.Worker.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AsyncApi.Worker
{
    public class Client : IClient
    {
        private ClientConfiguration _configuration;

        public string Version => _configuration.Version;

        public void Init(ClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<JToken> InvokeMethodObject(string method, object methodParams)
        {
            var job = new JObject
            {
                ["jsonrpc"] = _configuration.Version,
                ["id"] = "1",
                ["method"] = method,
                ["params"] = methodParams != null ? new JArray(JToken.FromObject(methodParams)) : new JArray()
            };

            var url = $"http://{_configuration.Address}:{_configuration.Port}/{_configuration.Endpoint}";
            var result = await SendRequest(url, _configuration.User, _configuration.Password, job);

            return result;
        }

        public async Task<JToken> InvokeMethodParams(string method, params object[] methodParams)
        {
            var job = new JObject
            {
                ["jsonrpc"] = _configuration.Version,
                ["id"] = "1",
                ["method"] = method
            };

            if (methodParams != null && methodParams.Length > 0)
            {
                var props = new JArray();

                foreach (var p in methodParams)
                {
                    props.Add(p);
                }

                job.Add(new JProperty("params", props));
            }
            else
            {
                job["params"] = new JArray();
            }

            var url = $"http://{_configuration.Address}:{_configuration.Port}/{_configuration.Endpoint}";
            var result = await SendRequest(url, _configuration.User, _configuration.Password, job);

            return result;
        }

        public async Task<string> SendRequest(HttpMethodType methodType, object methodParams = null, Dictionary<string, string> headers = null)
        {
            HttpWebRequest webRequest;
            var url = $"http://{_configuration.Address}:{_configuration.Port}/{_configuration.Endpoint}";

            switch (methodType)
            {
                case HttpMethodType.GET:
                    url += methodParams != null ? "?" + GetQueryString(methodParams) : "";
                    webRequest = (HttpWebRequest)WebRequest.Create(url);
                    webRequest.Method = "GET";
                    break;
                case HttpMethodType.POST:
                    webRequest = (HttpWebRequest)WebRequest.Create(url);
                    webRequest.Method = "POST";
                    break;
                default:
                {
                    throw new InternalServerErrorException("Undefined HTTP method");
                }
            }

            if (headers != null && headers.Any())
            {
                foreach (var header in headers)
                {
                    webRequest.Headers.Add(header.Key, header.Value);
                }
            }

            var data = methodParams != null ? JsonConvert.SerializeObject(methodParams) : "";
            return await SendRequest(webRequest, data);
        }

        private async Task<string> SendRequest(string url, string user, string password, JObject obj)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "POST";

            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(password))
            {
                webRequest.Headers[HttpRequestHeader.Authorization] =
                    $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{password}"))}";
            }

            var data = JsonConvert.SerializeObject(obj);

            return await SendRequest(webRequest, data);
        }

        private async Task<string> SendRequest(HttpWebRequest webRequest, string data = "")
        {
            webRequest.ContentType = "application/json";

            if (!string.IsNullOrEmpty(data))
            {
                var byteArray = Encoding.UTF8.GetBytes(data);
                //webRequest.ContentLength = byteArray.Length;
                webRequest.Headers[HttpRequestHeader.ContentLength] = byteArray.Length.ToString();

                using (var stream = await webRequest.GetRequestStreamAsync())
                {
                    await stream.WriteAsync(byteArray, 0, byteArray.Length);
                }
            }

            try
            {
                using (var response = await webRequest.GetResponseAsync())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var streamReader = new StreamReader(stream))
                        {
                            var readData = await streamReader.ReadToEndAsync();
                            return readData;
                        }
                    }
                }
            }
            catch (WebException webException)
            {
                if (webException.Response == null)
                {
                    throw new InternalServerErrorException("server not found");
                }
                else
                {
                    using (var stream = webException.Response.GetResponseStream())
                    {
                        using (var streamReader = new StreamReader(stream))
                        {
                            var readData = await streamReader.ReadToEndAsync();
                            throw new InternalServerErrorException($"\"error\": \"{readData}\"");
                        }
                    }
                }
            }
        }

        private string GetQueryString(object obj)
        {
            var properties = from p in obj.GetType().GetProperties() where p.GetValue(obj, null) != null select p.Name + "=" + p.GetValue(obj, null).ToString();
            return string.Join("&", properties.ToArray());
        }
    }
}
