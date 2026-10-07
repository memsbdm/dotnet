using Starter.Domain.ValueObjects;

namespace Starter.Application.Messaging;

public sealed record EmailMessage(
    Email Recipient,
    string Subject,
    string TextBody,
    string? HtmlBody = null);
