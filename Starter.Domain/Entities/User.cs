using Starter.Domain.ValueObjects;

namespace Starter.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Email Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;

    public User(Email email, string passwordHash)
    {
        Id = Guid.CreateVersion7();
        CreatedAt = DateTimeOffset.UtcNow;
        Email = email;
        PasswordHash = passwordHash;
    }

    private User() { } // For EF Core
}
