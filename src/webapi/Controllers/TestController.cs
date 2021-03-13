using Microsoft.AspNetCore.Mvc;
using Serilog;
using SerilogTimings.Extensions;
using System;
using System.Threading.Tasks;
namespace CasCap.Controllers
{
    /// <summary>
    /// Note: This controller is injecting the Serilog Logger implementation NOT the Microsoft abstraction!
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        readonly ILogger _logger;

        public TestController(ILogger logger)
        {
            _logger = logger?.ForContext<TestController>() ?? throw new ArgumentNullException(nameof(_logger));
        }

        [HttpGet]
        public async Task<ActionResult<WeatherForecast>> Get()
        {
            _logger.Information("web api controller {controllerName} method {methodName} hit", nameof(WeatherForecastController), nameof(Get));

            using (_logger.TimeOperation("Retrieving weather forecast id {id} from database", 123))
            {
                // Timed block of code goes here
                await Task.Delay(500);
            }

            return Ok(new WeatherForecast { Date = DateTime.UtcNow, TemperatureC = 25, Summary = "lovely outside!" });
        }
    }
}