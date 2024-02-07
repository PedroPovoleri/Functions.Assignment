using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Functions.Assignment.Function;
using Functions.Assignment.Function.Services.PostManEcho;
using Functions.Assignment.Function.Services.ServiceBus;
using Functions.Assignment.Function.Services.WeatherApi;
using Functions.Assignment.Model;

namespace Functions.Assignment.Test.Functions
{
    public class ServiceBusMessageActionFunctionTest
    {

        [Fact]
        [Trait("UnitTest", "Test Function")]
        public async Task ServiceBusMessageActionsFunction_ValidCity_ProcessesSuccessfully()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<ServiceBusFunc>>();
            var weatherApiClientMock = new Mock<IWeatherApiClient>();
            var postManEchoClientMock = new Mock<IPostManEchoClient>();
            var serviceBusQueueResponseMock = new Mock<IServiceBusQueueResponse>();

            var message = ServiceBusModelFactory.ServiceBusReceivedMessage(new BinaryData("{\"CityName\":\"ValidCity\"}"));
            var weatherApiResponse = new WeatherApiResponse() { Current = new Current(), Location = new Location() };
            var function = new ServiceBusFunc(weatherApiClientMock.Object, postManEchoClientMock.Object, serviceBusQueueResponseMock.Object, loggerMock.Object);


            // Setup
            weatherApiClientMock.Setup(x => x.GetCityWeather(It.IsAny<string>())).ReturnsAsync(weatherApiResponse);
            postManEchoClientMock.Setup(x => x.SendPost(weatherApiResponse)).Returns(Task.CompletedTask);
            serviceBusQueueResponseMock.Setup(x => x.CreateMessageInTheQueue(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            await function.ServiceBusMessageActionsFunction(message);

            // Assert
            weatherApiClientMock.Verify(x => x.GetCityWeather(It.IsAny<string>()), Times.AtLeastOnce, "GetCityWeather should be called at least once");
            postManEchoClientMock.Verify(x => x.SendPost(It.IsAny<WeatherApiResponse>()), Times.AtLeastOnce, "SendPost should be called at least once");
            serviceBusQueueResponseMock.Verify(x => x.CreateMessageInTheQueue(It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce, "CreateMessageInTheQueue should be called at least once");
        }

        [Fact]
        [Trait("UnitTest", "Test Function")]
        public async Task ServiceBusMessageActionsFunction_InvalidCity_LogsValidationError()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<ServiceBusFunc>>();
            var weatherApiClientMock = new Mock<IWeatherApiClient>();
            var postManEchoClientMock = new Mock<IPostManEchoClient>();
            var serviceBusQueueResponseMock = new Mock<IServiceBusQueueResponse>();

            var message = ServiceBusModelFactory.ServiceBusReceivedMessage(new BinaryData("{\"CityName\":\"\"}"));

            var function = new ServiceBusFunc(weatherApiClientMock.Object, postManEchoClientMock.Object, serviceBusQueueResponseMock.Object, loggerMock.Object);

            // Act
            await function.ServiceBusMessageActionsFunction(message);

            // Assert
            serviceBusQueueResponseMock.Verify(x => x.CreateMessageInTheQueue(It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce, "CreateMessageInTheQueue should be called at least once");
        }

        [Fact]
        [Trait("UnitTest", "Test Function")]
        public void ServiceBusMessageActionsResponseFunction_LogsCorrectly()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<ServiceBusFunc>>();
            var weatherApiClientMock = new Mock<IWeatherApiClient>();
            var postManEchoClientMock = new Mock<IPostManEchoClient>();
            var serviceBusQueueResponseMock = new Mock<IServiceBusQueueResponse>();
            serviceBusQueueResponseMock.Setup(x => x.CreateMessageInTheQueue(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);


            var optionsMock = new Mock<IOptions<LoggerFilterOptions>>();
            optionsMock.Setup(x => x.Value).Returns(new LoggerFilterOptions());

            var function = new ServiceBusFunc(weatherApiClientMock.Object, postManEchoClientMock.Object, serviceBusQueueResponseMock.Object, loggerMock.Object);

            var messageBody = "{\"LogType\":2,\"Message\":\"Test success log\"}";
            var message = ServiceBusModelFactory.ServiceBusReceivedMessage(new BinaryData(messageBody));

            // Act
            function.ServiceBusMessageActionsResponseFunction(message);

            // Assert
            Assert.Single(loggerMock.Invocations);
        }
    }
}
