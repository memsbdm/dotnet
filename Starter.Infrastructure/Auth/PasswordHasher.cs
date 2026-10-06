using Microsoft.AspNetCore.Identity;
using Starter.Application.Abstractions;

namespace Starter.Infrastructure.Authentication;

public sealed class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<object> _passwordHasher = new();

    public string Hash(string password)
    {
        return _passwordHasher.HashPassword(null!, password);
    }

    public bool Verify(string password, string hashedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(null!, hashedPassword, password);
        return result != PasswordVerificationResult.Failed;
    }
}
