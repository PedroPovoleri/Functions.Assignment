
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using System.Text.Json;
using Functions.Assignment.Function.Services.WeatherApi;
using Functions.Assignment.Model;

namespace Functions.Assignment.Test.WeatherApi
{

    public class WeatherApiClientTest
    {
        private readonly Dictionary<string, string> inMemorySettings;

        public WeatherApiClientTest()
        {
            inMemorySettings = new Dictionary<string, string> {
                    {"weatherapi-key", "AnyValidKey"},
                    {"weatherapi-host", "http://anyUrl.com/"} };
        }

        [Fact]
        [Trait("UnitTest", "Test WeatherApiClient")]
        public async Task GetCityWeather_ValidRequest_ReturnsApiResponse()
        {
            // Arrange
            var clientHandler = new MockHttpMessageHandler();
            var location = new Location
            {
                Name = "Rio de Janeiro"
            };
            var responseMock = new WeatherApiResponse
            {
                Location = location,
                Current = new Current()
            };
            var query = "Rio de Janeiro";

            clientHandler.When($"*")
                   .Respond("application/json", JsonSerializer.Serialize(responseMock));

            var loggerMock = new Mock<ILogger<WeatherApiClient>>();

            IConfiguration configuration = new ConfigurationBuilder()
                            .AddInMemoryCollection(inMemorySettings)
                            .Build();

            var weatherApiClient = new WeatherApiClient(clientHandler.ToHttpClient(), loggerMock.Object, configuration);

            // Act
            var result = await weatherApiClient.GetCityWeather(query);

            // Assert
            Assert.NotNull(result);
        }
    }
}
