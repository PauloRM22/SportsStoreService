using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace SportsStoreService.Extensions
{
    public static class OpenTelemetryExtensions
    {
        public static IServiceCollection AddObservability(
            this IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment environment)
        {
            var serviceName = configuration["OTel:ServiceName"] ?? "sportsstore-api";
            var serviceVersion = configuration["OTel:ServiceVersion"] ?? "1.0.0";
            var otlpEndpointUrl = configuration["OTel:Exporter:OtlpEndpoint"] ?? "http://datadog:4317";

            services.AddOpenTelemetry()
                .WithTracing(tracer => tracer
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(serviceName: serviceName, serviceVersion: serviceVersion)
                            .AddAttributes(new[]
                            {
                                new KeyValuePair<string, object>(
                                    "deployment.environment", environment.EnvironmentName)
                            })
                    )
                    .AddAspNetCoreInstrumentation(o => o.RecordException = true)
                    .AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation(o =>
                    {
                        o.SetDbStatementForText = true;
                        o.RecordException = true;
                    })
                    .AddOtlpExporter(o =>
                    {
                        o.Endpoint = new Uri(otlpEndpointUrl);
                    })
                );

            return services;
        }
    }
}