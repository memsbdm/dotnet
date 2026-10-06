using Starter.Domain.Exceptions;

namespace Starter.Domain.ValueObjects;

public class Email
{
    public string Value { get; }
    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {

            throw new DomainException("Email cannot be empty.");
        }

        value = value.Trim();

        if (!value.Contains('@'))
        {
            throw new DomainException("Email must contain '@' symbol.");
        }

        var idx = value.IndexOf('@');
        if (idx == 0 || idx == value.Length - 1)
        {
            throw new DomainException("Email must have characters before and after '@' symbol.");
        }

        Value = value.ToLowerInvariant();
    }
}
