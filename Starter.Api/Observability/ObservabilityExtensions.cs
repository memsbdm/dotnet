using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Starter.Api.Observability;

public static class ObservabilityExtensions
{
    private const string ServiceName = "Starter.Api";

    public static WebApplicationBuilder AddObservability(this WebApplicationBuilder builder)
    {
        var endpoint = builder.Configuration.GetValue<Uri>("OpenTelemetry:OtlpEndpoint");
        if (endpoint is null)
        {
            return builder;
        }

        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(ServiceName);

        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.SetResourceBuilder(resourceBuilder);
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
            logging.AddOtlpExporter(exporter => exporter.Endpoint = endpoint);
        });

        builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(ServiceName))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter(exporter => exporter.Endpoint = endpoint))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter(exporter => exporter.Endpoint = endpoint));

        return builder;
    }
}
