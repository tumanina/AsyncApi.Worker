namespace AsyncApi.Worker.MessageBroker
{
    public interface ISender
    {
        void SendMessage(string queueName, string message);
    }
}
