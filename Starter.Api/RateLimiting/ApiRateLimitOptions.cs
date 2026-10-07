namespace Starter.Api.RateLimiting;

public sealed class ApiRateLimitOptions
{
    public const string SectionName = "RateLimiting";

    public required SlidingWindowOptions Global { get; init; }
}

public sealed class SlidingWindowOptions
{
    public required int PermitLimit { get; init; }
    public required int WindowSeconds { get; init; }
    public required int SegmentsPerWindow { get; init; }
}
