using Microsoft.EntityFrameworkCore;
using Npgsql;
using Starter.Application.Abstractions;
using Starter.Domain.Entities;
using Starter.Domain.ValueObjects;
using Starter.Infrastructure.Persistence;

namespace Starter.Infrastructure.Repositories;

public sealed class UserRepository(AppDbContext dbContext) : IUserRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<bool> IsEmailTaken(Email email, CancellationToken cancellationToken)
    {
        return await _dbContext.Users.AnyAsync(
            u => u.Email.Value == email.Value,
            cancellationToken);
    }

    public async Task<bool> TryCreateUser(User user, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Users.AddAsync(user, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException exception) when (
            exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation
            })
        {
            _dbContext.Entry(user).State = EntityState.Detached;
            return false;
        }
    }

    public async Task<User?> GetUserByEmail(Email email, CancellationToken cancellationToken)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(
            u => u.Email.Value == email.Value,
            cancellationToken);
    }

    public async Task<User?> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(
            u => u.Id == id,
            cancellationToken);
    }
}
