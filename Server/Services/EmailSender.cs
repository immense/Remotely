using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Remotely.Server.Services;

public interface IEmailSenderEx
{
    Task<bool> SendEmailAsync(string email, string replyTo, string subject, string htmlMessage, string? organizationID = null);
    Task<bool> SendEmailAsync(string email, string subject, string htmlMessage, string? organizationID = null);
}

public class EmailSender : IEmailSender
{
    public EmailSender(IEmailSenderEx emailSenderEx)
    {
        EmailSenderEx = emailSenderEx;
    }

    private IEmailSenderEx EmailSenderEx { get; }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        return EmailSenderEx.SendEmailAsync(email, subject, htmlMessage, string.Empty);
    }
}

public class EmailSenderEx : IEmailSenderEx
{
    private readonly IApplicationConfig _appConfig;
    private readonly ILogger<EmailSenderEx> _logger;

    public EmailSenderEx(
        IApplicationConfig appConfig,
        ILogger<EmailSenderEx> logger)
    {
        _appConfig = appConfig;
        _logger = logger;
    }
    public async Task<bool> SendEmailAsync(
        string toEmail, 
        string replyTo, 
        string subject, 
        string htmlMessage, 
        string? organizationID = null)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_appConfig.SmtpDisplayName, _appConfig.SmtpEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.ReplyTo.Add(MailboxAddress.Parse(replyTo));
            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = htmlMessage
            };

            using var client = new SmtpClient();
            
            if (!string.IsNullOrWhiteSpace(_appConfig.SmtpLocalDomain))
            {
                client.LocalDomain = _appConfig.SmtpLocalDomain;
            }

            client.CheckCertificateRevocation = _appConfig.SmtpCheckCertificateRevocation;

            await client.ConnectAsync(_appConfig.SmtpHost, _appConfig.SmtpPort);

            if (!string.IsNullOrWhiteSpace(_appConfig.SmtpUserName) &&
                !string.IsNullOrWhiteSpace(_appConfig.SmtpPassword))
            {
                await client.AuthenticateAsync(_appConfig.SmtpUserName, _appConfig.SmtpPassword);
            }

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email successfully sent to {toEmail}.  Subject: \"{subject}\".", toEmail, subject);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while sending email.");
            return false;
        }
    }

    public Task<bool> SendEmailAsync(string email, string subject, string htmlMessage, string? organizationID = null)
    {
        return SendEmailAsync(email, _appConfig.SmtpEmail, subject, htmlMessage, organizationID);
    }
}
