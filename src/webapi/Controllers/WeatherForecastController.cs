using CasCap.Extensions;
using Microsoft.AspNetCore.Mvc;
namespace CasCap.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    static readonly string[] Summaries = new[]
    {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

    readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger) => _logger = logger;

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        _ = Environment.MachineName.TestStaticLogger();
        _logger.LogInformation("web api controller {controllerName} method {methodName} hit", nameof(WeatherForecastController), nameof(Get));
        var rng = new Random();
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = rng.Next(-20, 55),
            Summary = Summaries[rng.Next(Summaries.Length)]
        })
        .ToArray();
    }
}