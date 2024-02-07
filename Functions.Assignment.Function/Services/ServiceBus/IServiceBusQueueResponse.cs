namespace Functions.Assignment.Function.Services.ServiceBus
{
    public interface IServiceBusQueueResponse
    {
        Task CreateMessageInTheQueue(string queueName,  string message);
    }
}
    