using System;

namespace AsyncApi.Worker
{
    public class InternalServerErrorException : HttpException
    {
        public InternalServerErrorException() : base(409, "Ресурс уже существует.")
        {
        }

        public InternalServerErrorException(string message) : base(409, message)
        {
        }

        public InternalServerErrorException(Exception exception) : base(409, "Ресурс уже существует.", exception)
        {
        }

        public InternalServerErrorException(string message, Exception exception) : base(409, message, exception)
        {
        }
    }
}
