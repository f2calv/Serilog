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
    public class SerilogController : ControllerBase
    {
        readonly ILogger _logger;
        readonly IDiagnosticContext _diagnosticContext;

        public SerilogController(ILogger logger, IDiagnosticContext diagnosticContext)
        {
            _logger = logger?.ForContext<SerilogController>() ?? throw new ArgumentNullException(nameof(_logger));
            _diagnosticContext = diagnosticContext ?? throw new ArgumentNullException(nameof(diagnosticContext));
        }

        [HttpGet]
        public async Task<ActionResult<WeatherForecast>> Get()
        {
            // The request completion event will carry this property
            _diagnosticContext.Set("CatalogLoadTime", 1423);

            _logger
                .ForContext(nameof(Environment.MachineName), Environment.MachineName)//example use of ForContext to include properties and have terser messages
                .Information("web api controller {controllerName} method {methodName} hit", nameof(WeatherForecastController), nameof(Get));

            using (_logger.TimeOperation("Retrieving weather forecast id {id} from database", 123))
            {
                // Timed block of code goes here
                await Task.Delay(500);
            }

            return Ok(new WeatherForecast { Date = DateTime.UtcNow, TemperatureC = 25, Summary = "lovely outside!" });
        }
    }
}