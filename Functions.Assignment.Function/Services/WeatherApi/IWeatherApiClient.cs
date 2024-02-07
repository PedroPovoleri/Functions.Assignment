using Functions.Assignment.Model;

namespace Functions.Assignment.Function.Services.WeatherApi
{
    public interface IWeatherApiClient
    {
         Task<WeatherApiResponse> GetCityWeather(string query);
    }
}
