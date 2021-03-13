using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Linq;
namespace CasCap.Extensions
{
    public static class Helpers
    {
        public static string GetCorrelationId(this HttpContext httpContext)
        {
            httpContext.Request.Headers.TryGetValue("CC-Correlation-Id", out StringValues correlationId);
            return correlationId.FirstOrDefault() ?? httpContext.TraceIdentifier;
        }
    }
}