using System.Net;
using System.Net.Http.Json;
using Starter.IntegrationTests.Infrastructure;

namespace Starter.IntegrationTests.Auth;

[Collection(PostgreSqlTestGroup.Name)]
public sealed class AuthenticationTests(PostgreSqlFixture postgreSql) : IAsyncLifetime, IDisposable
{
    private readonly StarterApiFactory _factory = new(postgreSql);
    private HttpClient _client = null!;
    private bool _disposed;

    public async Task InitializeAsync()
    {
        await _factory.InitializeDatabaseAsync();
        _client = _factory.CreateClient();
    }

    public Task DisposeAsync()
    {
        Dispose();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _client?.Dispose();
        _factory.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GivenValidCredentialsWhenLoggingInThenEmailIsSent()
    {
        // Given
        var email = $"login-{Guid.NewGuid():N}@example.com";
        var credentials = new { Email = email, Password = "secret123" };
        var registerResponse = await _client.PostAsJsonAsync("/register", credentials);
        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        // When
        var loginResponse = await _client.PostAsJsonAsync("/login", credentials);

        // Then
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var message = Assert.Single(_factory.EmailSender.Messages);
        Assert.Equal(email, message.Recipient.Value);
        Assert.Equal("Successful login", message.Subject);
    }

    [Fact]
    public async Task GivenExistingEmailWhenRegisteringAgainThenConflictIsReturned()
    {
        // Given
        var credentials = new
        {
            Email = $"duplicate-{Guid.NewGuid():N}@example.com",
            Password = "secret123"
        };
        var firstResponse = await _client.PostAsJsonAsync("/register", credentials);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        // When
        var secondResponse = await _client.PostAsJsonAsync("/register", credentials);

        // Then
        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
    }
}
