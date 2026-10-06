using Starter.Application.Abstractions;
using Starter.Domain.Entities;
using Starter.Domain.ValueObjects;

namespace Starter.Application.Features.Auth.Register;

public class RegisterHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
{
    public async Task<RegisterResult> Handle(
        RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var email = new Email(command.Email);

        if (await userRepository.IsEmailTaken(email, cancellationToken))
        {
            return RegisterResult.EmailAlreadyTaken;
        }

        var password = new Password(command.Password);
        var hashedPassword = passwordHasher.Hash(password.Value);
        var user = new User(email, hashedPassword);

        if (!await userRepository.TryCreateUser(user, cancellationToken))
        {
            return RegisterResult.EmailAlreadyTaken;
        }

        return RegisterResult.Success;
    }
}
