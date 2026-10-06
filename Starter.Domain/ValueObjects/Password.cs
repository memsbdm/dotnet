using Starter.Domain.Exceptions;

namespace Starter.Domain.ValueObjects;

public class Password
{
    public string Value { get; }
    public Password(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("Password cannot be empty.");
        }

        if (value.Length < 8)
        {
            throw new DomainException("Password must be at least 8 characters long.");
        }

        if (value.Length > 32)
        {
            throw new DomainException("Password must be at most 32 characters long.");
        }

        Value = value;
    }
}
