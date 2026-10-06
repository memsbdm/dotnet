using Starter.Domain.Entities;

namespace Starter.Application.Features.Auth.Login;

public class LoginResult
{
    public bool Success { get; init; }
    public User? User { get; init; }
}
