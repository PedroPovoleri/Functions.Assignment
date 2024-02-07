using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Functions.Assignment.Function.Services.PostManEcho;
using Functions.Assignment.Function.Services.ServiceBus;
using Functions.Assignment.Function.Services.WeatherApi;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(c =>
    {
        c.AddEnvironmentVariables();
        var config = c.Build();
    })
    .ConfigureServices(s => s.AddHttpClient())
    .ConfigureServices(s => s.AddTransient<IWeatherApiClient, WeatherApiClient>())
    .ConfigureServices(s => s.AddTransient<IServiceBusQueueResponse, ServiceBusQueueResponse>())
    .ConfigureServices(s => s.AddTransient<IPostManEchoClient, PostManEchoClient>())
    .Build();

host.Run();
