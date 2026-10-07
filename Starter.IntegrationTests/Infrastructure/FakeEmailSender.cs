using System.Collections.Concurrent;
using Starter.Application.Abstractions;
using Starter.Application.Messaging;

namespace Starter.IntegrationTests.Infrastructure;

public sealed class FakeEmailSender : IEmailSender
{
    private readonly ConcurrentQueue<EmailMessage> _messages = new();

    public IReadOnlyCollection<EmailMessage> Messages => _messages.ToArray();

    public Task SendAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        _messages.Enqueue(message);
        return Task.CompletedTask;
    }
}
