namespace Starter.IntegrationTests.Infrastructure;

[CollectionDefinition(Name)]
public sealed class PostgreSqlTestGroup : ICollectionFixture<PostgreSqlFixture>
{
    public const string Name = "PostgreSQL";
}
