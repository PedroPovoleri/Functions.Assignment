using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Functions.Assignment.Function.Services.ServiceBus;

namespace Functions.Assignment.Test.ServiceBusTest
{
    public class ServiceBusQueueResponseTest
    {
        private readonly Dictionary<string, string> inMemorySettings;

        public ServiceBusQueueResponseTest()
        {
            inMemorySettings = new Dictionary<string, string> { { "ServiceBusConnectionString", "ValidConnectionString" } };
        }

        [Fact]
        [Trait("UnitTest", "ServiceBusQueueResponse")]
        public async Task CreateMessageInTheQueue_ValidRequest_SuccessfullySendsMessage()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<ServiceBusQueueResponse>>();

            // Set up in-memory configuration

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var serviceBusClientMock = new Mock<ServiceBusClient>();
            var senderMock = new Mock<ServiceBusSender>();

            serviceBusClientMock
                .Setup(c => c.CreateSender(It.IsAny<string>()))
                .Returns(senderMock.Object);

            senderMock
                .Setup(s => s.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var serviceBusQueueResponse = new ServiceBusQueueResponse(serviceBusClientMock.Object, loggerMock.Object, configuration);

            // Act
            await serviceBusQueueResponse.CreateMessageInTheQueue("fakeQueue", "testMessage");

            // Assert
            // Verify that SendMessageAsync was called with the correct message
            senderMock.Verify(s => s.SendMessageAsync(It.Is<ServiceBusMessage>(m => m.Body.ToString() == "testMessage"), It.IsAny<CancellationToken>()), Times.Once);

            // Verify that DisposeAsync was called on sender and client
            senderMock.Verify(s => s.DisposeAsync(), Times.Once);
            serviceBusClientMock.Verify(c => c.DisposeAsync(), Times.Once);
        }
    }
}
