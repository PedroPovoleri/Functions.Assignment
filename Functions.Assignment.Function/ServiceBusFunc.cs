using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;
using Functions.Assignment.Function.Helper;
using Functions.Assignment.Function.Services.PostManEcho;
using Functions.Assignment.Function.Services.ServiceBus;
using Functions.Assignment.Function.Services.WeatherApi;

namespace Functions.Assignment.Function
{
    public class ServiceBusFunc
    {
        private readonly ILogger<ServiceBusFunc> _logger;
        private readonly IWeatherApiClient _weatherApiClient;
        private readonly IPostManEchoClient _postManEchoClient;
        private readonly IServiceBusQueueResponse _serviceBusQueueResponse;

        public record CityRequest(string CityName);
        private record ProcessStatusRequest(LogType LogType, string Message);

        public ServiceBusFunc(IWeatherApiClient weatherApiClient,
                              IPostManEchoClient postManEchoClient,
                              IServiceBusQueueResponse serviceBusQueueResponse,
                              ILogger<ServiceBusFunc> logger)
        {
            _logger = logger;
            _weatherApiClient = weatherApiClient;
            _postManEchoClient = postManEchoClient;
            _serviceBusQueueResponse = serviceBusQueueResponse;
        }


        [Function("ServiceBusMessageActionsFunction")]
        public async Task ServiceBusMessageActionsFunction(
        [ServiceBusTrigger("assignment_queue", Connection = "ServiceBusConnectionString")]
        ServiceBusReceivedMessage message)
        {
            try
            {                
                var stpwtch = Stopwatch.StartNew();
                var city = JsonSerializer.Deserialize<CityRequest>(message.Body.ToString());
                if (city.IsValid())
                {
                    await Process(stpwtch, city);
                }
                else
                {
                    await CreateMessageLog(LogType.ValidationError, $" Invalid city name: {city.CityName}");
                }
            }
            catch (Exception ex)
            {
                await CreateMessageLog(LogType.Exception, ex.Message);
            }
        }

        [Function("ServiceBusMessageActionsResponseFunction")]
        public void ServiceBusMessageActionsResponseFunction(
        [ServiceBusTrigger("assignment_queue_response", Connection = "ServiceBusConnectionString")]
        ServiceBusReceivedMessage message)
        {
            var logToWrite = JsonSerializer.Deserialize<ProcessStatusRequest>(message.Body.ToString());

            switch (logToWrite.LogType)
            {
                case LogType.Success: _logger.LogInformation(logToWrite.Message); break;
                case LogType.Exception: _logger.LogError(logToWrite.Message); break;
                case LogType.ValidationError: _logger.LogWarning(logToWrite.Message); break;
            }
        }

        //[Function("TimerFunction")]
        //public async Task Run([TimerTrigger("* * * * *")] TimerInfo timerInfo,
            //FunctionContext context)
        //{
            //await _serviceBusQueueResponse.CreateMessageInTheQueue("assignment_queue", "{ \"CityName\":\"\"}");
        //}

        private async Task CreateMessageLog(LogType logType, string message)
        {
            var validationError = new ProcessStatusRequest(logType, message);
            await _serviceBusQueueResponse.CreateMessageInTheQueue("assignment_queue_response", JsonSerializer.Serialize(validationError));
        }

        private async Task Process(Stopwatch stpwtch, CityRequest city)
        {
            var weatherCity = await _weatherApiClient.GetCityWeather(city.CityName);

            if (weatherCity.IsValid())
            {
                await _postManEchoClient.SendPost(weatherCity);
                await CreateMessageLog(LogType.Success, $"The city: {city.CityName} was finish in time. {stpwtch.Elapsed.TotalSeconds}s");
            }
            else
            {
                await CreateMessageLog(LogType.ValidationError, $"Response from weather Api is not valid.");
            }
        }
    }
}
