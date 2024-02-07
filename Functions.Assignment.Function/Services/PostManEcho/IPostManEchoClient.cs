using Functions.Assignment.Model;

namespace Functions.Assignment.Function.Services.PostManEcho
{
    public interface IPostManEchoClient
    {
        Task SendPost(WeatherApiResponse weatherApiResponse);
    }
}
