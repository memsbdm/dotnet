using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Starter.Application.Abstractions;
using Starter.Infrastructure.Messaging;
using Starter.Infrastructure.Persistence;
using Starter.Infrastructure.Repositories;

namespace Starter.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Starter")
            ?? throw new InvalidOperationException("Connection string 'Starter' is required.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));
        services.AddScoped<IUserRepository, UserRepository>();
        services
            .AddOptions<EmailOptions>()
            .Bind(configuration.GetRequiredSection(EmailOptions.SectionName))
            .ValidateDataAnnotations()
            .Validate(
                options => string.IsNullOrWhiteSpace(options.Username) == string.IsNullOrWhiteSpace(options.Password),
                "Email username and password must either both be provided or both be omitted.")
            .ValidateOnStart();
        services.AddSingleton<IEmailSender, MailKitEmailSender>();

        return services;
    }
}
