using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Options;
using MimeKit;
using Studi.BLL.Models.Email;
using Studi.BLL.Models.Email.Base;
using Studi.BLL.Services.Interfaces;

namespace Studi.BLL.Services.Realizations;

public class SmtpEmailService : IEmailService
{
    private readonly SmtpOptions _options;
    private readonly ILoggerService _logger;

    public SmtpEmailService(IOptions<SmtpOptions> options, ILoggerService logger)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(MessageData messageData)
    {
        ArgumentNullException.ThrowIfNull(messageData);

        try
        {
            var message = messageData.ToMimeMessage();
            EnsureFrom(message);

            using var mailMessage = ToMailMessage(message);
            using var client = new SmtpClient(_options.Host, _options.Port)
            {
                EnableSsl = ShouldEnableSsl(_options.SecureSocketOptions)
            };

            if (!string.IsNullOrWhiteSpace(_options.UserName)
                && !string.IsNullOrWhiteSpace(_options.Password))
            {
                client.Credentials = new NetworkCredential(_options.UserName, _options.Password);
            }

            await client.SendMailAsync(mailMessage);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(messageData, $"Failed to send email via SMTP: {ex.Message}", ex.StackTrace);

            return false;
        }
    }

    private void EnsureFrom(MimeMessage message)
    {
        if (message.From.Count > 0)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_options.FromAddress))
        {
            throw new InvalidOperationException("SMTP from address is not configured.");
        }

        var fromName = string.IsNullOrWhiteSpace(_options.FromName)
            ? _options.FromAddress
            : _options.FromName;

        message.From.Add(new MailboxAddress(fromName, _options.FromAddress));
    }

    private static bool ShouldEnableSsl(string? option)
    {
        if (string.IsNullOrWhiteSpace(option))
        {
            return true;
        }

        return !string.Equals(option, "None", StringComparison.OrdinalIgnoreCase);
    }

    private static MailMessage ToMailMessage(MimeMessage message)
    {
        var mailMessage = new MailMessage();

        var from = message.From.Mailboxes.FirstOrDefault();
        if (from is not null)
        {
            mailMessage.From = new MailAddress(from.Address, from.Name);
        }

        foreach (var recipient in message.To.Mailboxes)
        {
            mailMessage.To.Add(new MailAddress(recipient.Address, recipient.Name));
        }

        mailMessage.Subject = message.Subject ?? string.Empty;
        mailMessage.SubjectEncoding = Encoding.UTF8;
        mailMessage.BodyEncoding = Encoding.UTF8;

        if (!string.IsNullOrWhiteSpace(message.HtmlBody))
        {
            if (!string.IsNullOrWhiteSpace(message.TextBody))
            {
                mailMessage.AlternateViews.Add(
                    AlternateView.CreateAlternateViewFromString(message.TextBody, Encoding.UTF8, "text/plain"));
            }

            mailMessage.AlternateViews.Add(
                AlternateView.CreateAlternateViewFromString(message.HtmlBody, Encoding.UTF8, "text/html"));

            mailMessage.Body = message.HtmlBody;
            mailMessage.IsBodyHtml = true;
        }
        else
        {
            mailMessage.Body = message.TextBody ?? string.Empty;
            mailMessage.IsBodyHtml = false;
        }

        return mailMessage;
    }
}
