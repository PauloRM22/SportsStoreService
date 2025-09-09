using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace SportsStoreService.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
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

                var factory = ctx.RequestServices.GetRequiredService<ProblemDetailsFactory>();

                (ProblemDetails problem, int statusCode) = CreateProblemDetails(factory, ctx, ex, traceId);

                ctx.Response.ContentType = "application/problem+json";
                ctx.Response.StatusCode = statusCode;
                await ctx.Response.WriteAsJsonAsync(problem);
            }
        }

        private (ProblemDetails problem, int statusCode) CreateProblemDetails(
            ProblemDetailsFactory factory,
            HttpContext ctx,
            Exception ex,
            string traceId)
        {
            switch (ex)
            {
                // 400 - BadRequest com ValidationProblemDetails (FluentValidation)
                case ValidationException fvEx:
                    var errors = fvEx.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                    var vpd = new ValidationProblemDetails(errors)
                    {
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                        Title = "One or more validation errors occurred.",
                        Status = StatusCodes.Status400BadRequest,
                        Instance = ctx.Request.Path,
                        Detail = _env.IsDevelopment() ? fvEx.Message : null
                    };
                    vpd.Extensions["traceId"] = traceId;
                    return (vpd, StatusCodes.Status400BadRequest);

                // 404 - Not Found
                case KeyNotFoundException:
                case { } e when e.GetType().Name == "NotFoundException":
                    var notFound = factory.CreateProblemDetails(
                        ctx,
                        statusCode: StatusCodes.Status404NotFound,
                        title: "Resource not found.",
                        type: "https://httpstatuses.com/404",
                        detail: _env.IsDevelopment() ? ex.Message : null,
                        instance: ctx.Request.Path);
                    notFound.Extensions["traceId"] = traceId;
                    return (notFound, StatusCodes.Status404NotFound);

                // 409 - Conflict (e-mail duplicado, concorrência, etc.)
                case { } e when e is InvalidOperationException && e.Message.Contains("conflict", StringComparison.OrdinalIgnoreCase):
                case { } e2 when e2.GetType().Name == "ConflictException":
                    var conflict = factory.CreateProblemDetails(
                        ctx,
                        statusCode: StatusCodes.Status409Conflict,
                        title: "Conflict.",
                        type: "https://httpstatuses.com/409",
                        detail: _env.IsDevelopment() ? ex.Message : null,
                        instance: ctx.Request.Path);
                    conflict.Extensions["traceId"] = traceId;
                    return (conflict, StatusCodes.Status409Conflict);

                // 401 - Unauthorized
                case UnauthorizedAccessException:
                    var unauthorized = factory.CreateProblemDetails(
                        ctx,
                        statusCode: StatusCodes.Status401Unauthorized,
                        title: "Unauthorized.",
                        type: "https://httpstatuses.com/401",
                        detail: _env.IsDevelopment() ? ex.Message : null,
                        instance: ctx.Request.Path);
                    unauthorized.Extensions["traceId"] = traceId;
                    return (unauthorized, StatusCodes.Status401Unauthorized);

                // 500 - Erro genérico com correlação
                default:
                    var internalErr = factory.CreateProblemDetails(
                        ctx,
                        statusCode: StatusCodes.Status500InternalServerError,
                        title: "An unexpected error occurred.",
                        type: "https://httpstatuses.com/500",
                        detail: _env.IsDevelopment() ? ex.Message : null,
                        instance: ctx.Request.Path);
                    internalErr.Extensions["traceId"] = traceId;
                    return (internalErr, StatusCodes.Status500InternalServerError);
            }
        }
    }
}