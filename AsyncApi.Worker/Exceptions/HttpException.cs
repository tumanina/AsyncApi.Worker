using System;

namespace AsyncApi.Worker
{
    public class HttpException : Exception
    {
        public int StatusCode { get; set; }

        public HttpException(int statusCode) : base($"An error occurred while running the request ({statusCode}).")
        {
            StatusCode = statusCode;
        }

        public HttpException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpException(int statusCode, string message, Exception innerException) : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
