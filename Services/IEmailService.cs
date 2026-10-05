namespace ClinicManagement.Services;

public interface IEmailService
{
    Task SendEmailAsync(
        string recipientEmail,
        string subject,
        string htmlBody);

    Task SendEmailWithAttachmentAsync(
        string recipientEmail,
        string subject,
        string htmlBody,
        byte[] attachmentBytes,
        string attachmentFileName,
        string attachmentContentType);
}