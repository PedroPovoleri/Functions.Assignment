using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.Http.Json;
using Functions.Assignment.Model;

namespace Functions.Assignment.Function.Services.WeatherApi
{
    public class WeatherApiClient : IWeatherApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WeatherApiClient> _logger;
        private readonly IConfiguration _config;
        private readonly string _url;
        private readonly string _apiKey;
        private readonly string _weatherapiHost;

        public WeatherApiClient(HttpClient httpClient, ILogger<WeatherApiClient> logger, IConfiguration config)
        {
            _httpClient = httpClient;
            _logger = logger;
            _config = config;
            _url = _config.GetValue<string>("weatherapi-host");
            _apiKey = _config.GetValue<string>("weatherapi-key");
            _weatherapiHost = _config.GetValue<string>("weatherapi-app");
        }

        public async Task<WeatherApiResponse> GetCityWeather(string query)
        {
            var stpWtch = Stopwatch.StartNew();
            _logger.LogInformation("Start request for the query: {query}", query);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_url}/current.json?q={query}"),
                Headers =
                {
                    { "X-RapidAPI-Key", _apiKey },
                    { "X-RapidAPI-Host", _weatherapiHost }
                }
            };
            _httpClient.BaseAddress = new Uri(_url);
            using var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            _logger.LogInformation("Sending reuqest..");
            var responseObject = await response.Content.ReadFromJsonAsync<WeatherApiResponse>();
            _logger.LogInformation("Fatch respose in {total} seconds.", stpWtch.Elapsed.TotalSeconds);
            return (responseObject is null ? new WeatherApiResponse() : responseObject);
        }
    }
}
