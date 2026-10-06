using Starter.Domain.Entities;
using Starter.Domain.ValueObjects;

namespace Starter.Application.Abstractions;

public interface IUserRepository
{
    Task<bool> IsEmailTaken(Email email, CancellationToken cancellationToken);
    Task<bool> TryCreateUser(User user, CancellationToken cancellationToken);
    Task<User?> GetUserByEmail(Email email, CancellationToken cancellationToken);
    Task<User?> GetUserById(Guid id, CancellationToken cancellationToken);
}
