using Microsoft.Extensions.Caching.Hybrid;

namespace Starter.Api.Caching;

public static class CachingExtensions
{
    public static IServiceCollection AddApiCaching(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("Connection string 'Redis' is required.");

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = connectionString;
            options.InstanceName = "starter:";
        });
        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(1),
                LocalCacheExpiration = TimeSpan.FromSeconds(15)
            };
        });

        return services;
    }
}
