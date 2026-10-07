using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Starter.Application.Abstractions;
using Starter.Application.Messaging;

namespace Starter.Infrastructure.Messaging;

public sealed class MailKitEmailSender(IOptions<EmailOptions> options) : IEmailSender
{
    private readonly EmailOptions _options = options.Value;

    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress(_options.FromName, _options.FromAddress));
        mimeMessage.To.Add(MailboxAddress.Parse(message.Recipient.Value));
        mimeMessage.Subject = message.Subject;
        mimeMessage.Body = new BodyBuilder
        {
            TextBody = message.TextBody,
            HtmlBody = message.HtmlBody
        }.ToMessageBody();

        using var client = new SmtpClient();

        await client.ConnectAsync(
            _options.Host,
            _options.Port,
            _options.UseSsl,
            cancellationToken);

        if (!string.IsNullOrWhiteSpace(_options.Username)
            && !string.IsNullOrWhiteSpace(_options.Password))
        {
            await client.AuthenticateAsync(
                _options.Username,
                _options.Password,
                cancellationToken);
        }

        await client.SendAsync(mimeMessage, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}
