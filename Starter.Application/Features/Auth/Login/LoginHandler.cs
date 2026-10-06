using Starter.Application.Abstractions;
using Starter.Domain.Entities;
using Starter.Domain.ValueObjects;

namespace Starter.Application.Features.Auth.Login;

public class LoginHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
{
    public async Task<LoginResult> Handle(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        var email = new Email(command.Email);
        var user = await userRepository.GetUserByEmail(email, cancellationToken);
        if (user == null)
        {
            return new LoginResult { Success = false };
        }

        if (!passwordHasher.Verify(command.Password, user.PasswordHash))
        {
            return new LoginResult { Success = false };
        }

        return new LoginResult { Success = true, User = user };
    }
}
