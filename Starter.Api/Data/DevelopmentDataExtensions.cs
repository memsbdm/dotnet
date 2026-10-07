using Microsoft.EntityFrameworkCore;
using Starter.Application.Features.Auth.Register;
using Starter.Infrastructure.Persistence;

namespace Starter.Api.Data;

public static class DevelopmentDataExtensions
{
    public static async Task InitialiseDevelopmentDataAsync(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return;
        }

        await using var scope = app.Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync(app.Lifetime.ApplicationStopping);

        var seedUser = app.Configuration.GetRequiredSection("SeedUser");
        var email = seedUser["Email"]
            ?? throw new InvalidOperationException("SeedUser:Email is required in Development.");
        var password = seedUser["Password"]
            ?? throw new InvalidOperationException("SeedUser:Password is required in Development.");

        var registerHandler = scope.ServiceProvider.GetRequiredService<RegisterHandler>();
        await registerHandler.Handle(
            new RegisterCommand(email, password),
            app.Lifetime.ApplicationStopping);
    }
}
