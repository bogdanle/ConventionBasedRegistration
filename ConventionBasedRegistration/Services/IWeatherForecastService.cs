using ConventionBasedRegistration.Models;

namespace ConventionBasedRegistration.Services;

public interface IWeatherForecastService
{
    Task<IEnumerable<WeatherForecast>> GetAsync();
}
