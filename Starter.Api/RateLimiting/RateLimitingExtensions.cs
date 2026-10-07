using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Starter.Api.RateLimiting;

public static class RateLimitingExtensions
{
    private static readonly SlidingWindowOptions LoginRateLimit = new()
    {
        PermitLimit = 5,
        WindowSeconds = 60,
        SegmentsPerWindow = 6
    };

    public static IServiceCollection AddApiRateLimiting(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var rateLimits = configuration
            .GetRequiredSection(ApiRateLimitOptions.SectionName)
            .Get<ApiRateLimitOptions>()
            ?? throw new InvalidOperationException("Rate limiting configuration is required.");

        Validate(rateLimits.Global, "RateLimiting:Global");

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = (context, cancellationToken) =>
                WriteRejectionResponse(context, rateLimits, cancellationToken);

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                CreatePartition(context, rateLimits.Global));

            options.AddPolicy(
                RateLimitPolicies.Login,
                context => CreatePartition(context, LoginRateLimit));
        });

        return services;
    }

    private static RateLimitPartition<string> CreatePartition(
        HttpContext context,
        SlidingWindowOptions options)
    {
        var partitionKey = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey,
            _ => new SlidingWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = options.PermitLimit,
                QueueLimit = 0,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                SegmentsPerWindow = options.SegmentsPerWindow,
                Window = TimeSpan.FromSeconds(options.WindowSeconds)
            });
    }

    private static async ValueTask WriteRejectionResponse(
        OnRejectedContext context,
        ApiRateLimitOptions options,
        CancellationToken cancellationToken)
    {
        var retryAfterSeconds = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter)
            ? Math.Max(1, (int)Math.Ceiling(retryAfter.TotalSeconds))
            : GetFallbackRetryAfterSeconds(context.HttpContext, options);

        context.HttpContext.Response.Headers.RetryAfter = retryAfterSeconds.ToString(CultureInfo.InvariantCulture);

        await Results.Problem(
            statusCode: StatusCodes.Status429TooManyRequests,
            title: "Too many requests.",
            detail: "The request rate limit has been exceeded. Try again later.")
            .ExecuteAsync(context.HttpContext);
    }

    private static void Validate(SlidingWindowOptions options, string sectionName)
    {
        if (options.PermitLimit <= 0
            || options.WindowSeconds <= 0
            || options.SegmentsPerWindow <= 0)
        {
            throw new InvalidOperationException($"{sectionName} values must be greater than zero.");
        }
    }

    private static int GetFallbackRetryAfterSeconds(
        HttpContext context,
        ApiRateLimitOptions options)
    {
        var globalSegmentSeconds = options.Global.WindowSeconds / options.Global.SegmentsPerWindow;
        var endpointPolicy = context.GetEndpoint()?
            .Metadata
            .GetMetadata<EnableRateLimitingAttribute>()?
            .PolicyName;

        if (endpointPolicy != RateLimitPolicies.Login)
        {
            return Math.Max(1, globalSegmentSeconds);
        }

        var loginSegmentSeconds = LoginRateLimit.WindowSeconds / LoginRateLimit.SegmentsPerWindow;
        return Math.Max(1, Math.Max(globalSegmentSeconds, loginSegmentSeconds));
    }
}
