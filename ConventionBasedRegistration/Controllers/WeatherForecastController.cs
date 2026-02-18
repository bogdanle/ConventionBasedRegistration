using ConventionBasedRegistration.Models;
using ConventionBasedRegistration.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConventionBasedRegistration.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IWeatherForecastService _forecastService;

    public WeatherForecastController(IWeatherForecastService forecastService)
    {
        _forecastService = forecastService;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        return await _forecastService.GetAsync();
    }
}
