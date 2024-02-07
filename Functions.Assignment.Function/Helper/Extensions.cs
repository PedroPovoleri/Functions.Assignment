using System.Text.RegularExpressions;
using Functions.Assignment.Model;
using static Functions.Assignment.Function.ServiceBusFunc;

namespace Functions.Assignment.Function.Helper
{
    public static class Extensions
    {
        public static bool IsValid(this WeatherApiResponse weatherApiResponse)
        {
            if (weatherApiResponse.Current is null)
                return false;
            if (weatherApiResponse.Location is null)
                return false;
            return true;
        }
        public static bool IsValid(this CityRequest cityRequest) {
            string cityNameRegex = @"^[a-zA-Z0-9' -]{1,100}$";
            return Regex.IsMatch(cityRequest.CityName, cityNameRegex);
        } 
    }
}
