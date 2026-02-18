using ConventionBasedRegistration.IoC;
using ConventionBasedRegistration.Models;

namespace ConventionBasedRegistration.Services;

[ContainerRegistration(ObjectLifetime.Scoped)]
public class WeatherForecastService : IWeatherForecastService
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    public async Task<IEnumerable<WeatherForecast>> GetAsync()
    {
        // Simulate a lengthy operation (e.g., database call, external API call)
        await Task.Delay(2000); // 2 second delay

        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }
}
