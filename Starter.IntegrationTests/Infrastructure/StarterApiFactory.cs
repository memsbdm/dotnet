using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Starter.Application.Abstractions;
using Starter.Infrastructure.Persistence;

namespace Starter.IntegrationTests.Infrastructure;

public sealed class StarterApiFactory(PostgreSqlFixture postgreSql) : WebApplicationFactory<Program>
{
    public FakeEmailSender EmailSender { get; } = new();

    public async Task InitializeDatabaseAsync()
    {
        _ = CreateClient();

        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .UseEnvironment("IntegrationTests")
            .UseSetting("ConnectionStrings:Starter", postgreSql.ConnectionString)
            .UseSetting("ConnectionStrings:Redis", "localhost:0,abortConnect=false")
            .UseSetting("Email:Host", "localhost")
            .UseSetting("Email:Port", "1025")
            .UseSetting("Email:FromAddress", "no-reply@starter.test")
            .UseSetting("Email:FromName", "Starter Tests")
            .UseSetting("Email:UseSsl", "false");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IEmailSender>();
            services.AddSingleton<IEmailSender>(EmailSender);
        });
    }
}
