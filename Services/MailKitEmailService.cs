using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Utils;

namespace ClinicManagement.Services;

public class MailKitEmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public MailKitEmailService(
        IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }

    public async Task SendEmailAsync(
        string recipientEmail,
        string subject,
        string htmlBody)
    {
        var message = new MimeMessage();

        message.From.Add(
            new MailboxAddress(
                _settings.FromName,
                _settings.FromEmail));

        message.To.Add(
            MailboxAddress.Parse(recipientEmail));

        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = htmlBody
        };

        message.Body = bodyBuilder.ToMessageBody();

        await SendMessageAsync(message);
    }

    public async Task SendEmailWithAttachmentAsync(
        string recipientEmail,
        string subject,
        string htmlBody,
        byte[] attachmentBytes,
        string attachmentFileName,
        string attachmentContentType)
    {
        var message = new MimeMessage();

        message.From.Add(
            new MailboxAddress(
                _settings.FromName,
                _settings.FromEmail));

        message.To.Add(
            MailboxAddress.Parse(recipientEmail));

        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = htmlBody
        };

        bodyBuilder.Attachments.Add(
            attachmentFileName,
            attachmentBytes,
            ContentType.Parse(attachmentContentType));

        message.Body = bodyBuilder.ToMessageBody();

        await SendMessageAsync(message);
    }

    private async Task SendMessageAsync(
        MimeMessage message)
    {
        using var smtpClient = new SmtpClient();

        var socketOption = _settings.UseSsl
            ? SecureSocketOptions.SslOnConnect
            : SecureSocketOptions.StartTls;

        await smtpClient.ConnectAsync(
            _settings.Host,
            _settings.Port,
            socketOption);

        await smtpClient.AuthenticateAsync(
            _settings.UserName,
            _settings.Password);

        await smtpClient.SendAsync(message);

        await smtpClient.DisconnectAsync(
            true);
    }
}