using System;

namespace AsyncApi.Worker.MessageBroker
{
    public static class ExceptionExtension
    {
        public static string InnerMessage(this Exception exception)
        {
            while (true)
            {
                if (exception.InnerException == null) return exception.Message;
                exception = exception.InnerException;
            }
        }
    }
}
