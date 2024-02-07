using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using Functions.Assignment.Model;

namespace Functions.Assignment.Function.Services.PostManEcho
{
    public class PostManEchoClient : IPostManEchoClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public PostManEchoClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task SendPost(WeatherApiResponse weatherApiResponse)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, _configuration.GetValue<string>("PostManUrl"));
            var content = new StringContent(JsonSerializer.Serialize(weatherApiResponse), Encoding.UTF8, "application/json");
            request.Content = content;
            using var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}
