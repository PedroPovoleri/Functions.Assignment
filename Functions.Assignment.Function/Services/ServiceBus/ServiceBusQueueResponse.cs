using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Functions.Assignment.Function.Services.ServiceBus
{
    public class ServiceBusQueueResponse : IServiceBusQueueResponse
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ServiceBusQueueResponse> _logger;
        private ServiceBusClient _serviceBusClient;
        public ServiceBusQueueResponse(ILogger<ServiceBusQueueResponse> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        internal ServiceBusQueueResponse(ServiceBusClient serviceBusQueueResponse, ILogger<ServiceBusQueueResponse> logger, IConfiguration configuration)
        {
            // Create a ServiceBusClient instance using a connection string
            if (_serviceBusClient is null)
                _serviceBusClient = new ServiceBusClient(_configuration.GetValue<string>("ServiceBusConnectionString"));

            _serviceBusClient = serviceBusQueueResponse;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task CreateMessageInTheQueue(string queueName, string message)
        {


            // Create a sender for the specified queue
            var sender = _serviceBusClient.CreateSender(queueName);

            // Create a message with the provided content
            var serviceBusMessage = new ServiceBusMessage(message);

            try
            {
                // Send the message to the queue
                await sender.SendMessageAsync(serviceBusMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError("Something goes wrong at the service bus: {ex}", ex.Message);
                throw;
            }
            finally
            {
                // Dispose of the sender to release resources
                await sender.DisposeAsync();
                // Dispose of the client if you are not using it anymore
                await _serviceBusClient.DisposeAsync();
            }
        }
    }
}
