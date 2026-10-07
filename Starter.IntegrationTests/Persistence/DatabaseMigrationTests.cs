using Microsoft.EntityFrameworkCore;
using Starter.Infrastructure.Persistence;
using Starter.IntegrationTests.Infrastructure;

namespace Starter.IntegrationTests.Persistence;

[Collection(PostgreSqlTestGroup.Name)]
public sealed class DatabaseMigrationTests(PostgreSqlFixture postgreSql)
{
    [Fact]
    public async Task MigrationsCanBeAppliedToEmptyDatabase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(postgreSql.ConnectionString)
            .Options;

        await using var dbContext = new AppDbContext(options);

        await dbContext.Database.MigrateAsync();

        Assert.True(await dbContext.Database.CanConnectAsync());
    }
}
