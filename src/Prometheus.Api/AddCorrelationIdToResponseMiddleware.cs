using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Prometheus.Api
{
    public class AddCorrelationIdToResponseMiddleware
    {
        private const string CorrelationIdHeaderName = "X-Correlation-ID";
        private readonly RequestDelegate _next;

        public AddCorrelationIdToResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            context
                .Response
                .Headers
                .Add(CorrelationIdHeaderName, context.TraceIdentifier);

            return _next(context);
        }
    }

    public class AddCorrelationIdToLogContextMiddleware
    {
        private readonly RequestDelegate _next;

        public AddCorrelationIdToLogContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            using (LogContext.PushProperty("CorrelationID", context.TraceIdentifier))
            {
                return _next(context);
            }
        }
    }
}