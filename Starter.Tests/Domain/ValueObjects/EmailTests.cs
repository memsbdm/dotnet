using Starter.Domain.Exceptions;
using Starter.Domain.ValueObjects;

namespace Starter.Tests.Domain.ValueObjects;

public sealed class EmailTests
{
    [Fact]
    public void ConstructorNormalizesEmail()
    {
        var email = new Email("  John.Doe@Example.COM  ");

        Assert.Equal("john.doe@example.com", email.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    [InlineData("@example.com")]
    [InlineData("john.doe@")]
    public void ConstructorRejectsInvalidEmail(string value)
    {
        Assert.Throws<DomainException>(() => new Email(value));
    }
}
