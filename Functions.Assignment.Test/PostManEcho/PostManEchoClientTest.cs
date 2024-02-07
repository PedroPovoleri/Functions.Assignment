using Microsoft.Extensions.Configuration;
using RichardSzalay.MockHttp;
using System.Text.Json;
using Functions.Assignment.Function.Services.PostManEcho;
using Functions.Assignment.Model;

namespace Functions.Assignment.Test.PostManEcho
{
    public class PostManEchoClientTest
    {
        private readonly Dictionary<string, string> inMemorySettings;

        public PostManEchoClientTest()
        {
            inMemorySettings = new Dictionary<string, string> {
                    {"PostManUrl", "http://localhost/post"} };
        }

        [Fact]
        [Trait("UnitTest", "Test PostManEchoClient")]
        public async Task SendPost_ValidRequest_SuccessfulResponse()
        {
            // Arrange
            var clientHandler = new MockHttpMessageHandler();
            var responseMock = new WeatherApiResponse
            {
                Location = new Location { Name = "TestCity" },
                Current = new Current()
            };

            clientHandler.When("*")
                   .Respond("application/json", JsonSerializer.Serialize(responseMock));

            var httpClient = new HttpClient(clientHandler);

            IConfiguration configuration = new ConfigurationBuilder()
                            .AddInMemoryCollection(inMemorySettings)
                            .Build();

            var postManEchoClient = new PostManEchoClient(httpClient, configuration);

            // Act
            await postManEchoClient.SendPost(responseMock);

            // Assert
            clientHandler.Expect("http://localhost/post");
        }
    }
}
