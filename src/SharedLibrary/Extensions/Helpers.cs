using CasCap.Common.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System.Linq;
namespace CasCap.Extensions
{
    public static class Helpers
    {
        static readonly ILogger _logger = ApplicationLogging.CreateLogger(nameof(Helpers));

        const string keyCorrelationId = "CC-Correlation-Id";

        public static string GetCorrelationId(this HttpContext httpContext)
        {
            if (httpContext.Request.Headers.TryGetValue(keyCorrelationId, out StringValues correlationId))
                _logger.LogDebug("CorrelationID value of {correlationId} is detected", correlationId);
            return correlationId.FirstOrDefault() ?? httpContext.TraceIdentifier;
        }

        public static string TestStaticLogger(this string input)
        {
            _logger.LogInformation("Input string is {input}", input);
            return input;
        }
    }
}