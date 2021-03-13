using CasCap.Extensions;
using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System.Threading.Tasks;
namespace CasCap.Middleware
{
    public class RequestLogContextMiddleware
    {
        readonly RequestDelegate _next;

        public RequestLogContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            using (LogContext.PushProperty("CorrelationId", context.GetCorrelationId()))
            {
                return _next.Invoke(context);
            }
        }
    }
}