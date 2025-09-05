using System.Net;
using System.Text.Json;

namespace SportsStoreService.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext ctx)
        {
            try
            {
                await _next(ctx);
            }
            catch (Exception ex)
            {
                var traceId = ctx.TraceIdentifier;
                _logger.LogError(ex, "Unhandled exception. TraceId={TraceId}", traceId);

                var (status, error) = Map(ex);
                ctx.Response.ContentType = "application/json";
                ctx.Response.StatusCode = (int)status;

                var payload = new
                {
                    traceId,
                    error,
                    message = ex.Message
                };

                await ctx.Response.WriteAsync(JsonSerializer.Serialize(payload));
            }
        }

        private static (HttpStatusCode status, string error) Map(Exception ex) =>
            ex switch
            {
                ArgumentException => (HttpStatusCode.UnprocessableEntity, "VALIDATION"),
                KeyNotFoundException => (HttpStatusCode.NotFound, "NOT_FOUND"),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "UNAUTHORIZED"),
                _ => (HttpStatusCode.InternalServerError, "INTERNAL")
            };
    }
}