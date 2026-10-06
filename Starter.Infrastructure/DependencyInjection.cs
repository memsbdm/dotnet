using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Starter.Application.Abstractions;
using Starter.Infrastructure.Persistence;
using Starter.Infrastructure.Repositories;

namespace Starter.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("Starter")));
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
