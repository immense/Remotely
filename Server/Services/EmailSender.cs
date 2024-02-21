using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using MimeKit;
using MimeKit.Text;
using NuGet.Configuration;
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
    private readonly IDataService _dataService;
    private readonly ILogger<EmailSenderEx> _logger;

    public EmailSenderEx(
        IDataService dataService,
        ILogger<EmailSenderEx> logger)
    {
        _dataService = dataService;
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
            var settings = await _dataService.GetSettings();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(settings.SmtpDisplayName, settings.SmtpEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.ReplyTo.Add(MailboxAddress.Parse(replyTo));
            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = htmlMessage
            };

            using var client = new SmtpClient();
            
            if (!string.IsNullOrWhiteSpace(settings.SmtpLocalDomain))
            {
                client.LocalDomain = settings.SmtpLocalDomain;
            }

            client.CheckCertificateRevocation = settings.SmtpCheckCertificateRevocation;

            await client.ConnectAsync(settings.SmtpHost, settings.SmtpPort);

            if (!string.IsNullOrWhiteSpace(settings.SmtpUserName) &&
                !string.IsNullOrWhiteSpace(settings.SmtpPassword))
            {
                await client.AuthenticateAsync(settings.SmtpUserName, settings.SmtpPassword);
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

    public async Task<bool> SendEmailAsync(string email, string subject, string htmlMessage, string? organizationID = null)
    {
        var settings = await _dataService.GetSettings();
        return await SendEmailAsync(email, settings.SmtpEmail, subject, htmlMessage, organizationID);
    }
}
public class EmailSenderFake(ILogger<EmailSenderFake> _logger) : IEmailSenderEx
{
    public Task<bool> SendEmailAsync(string email, string replyTo, string subject, string htmlMessage, string? organizationID = null)
    {
        _logger.LogInformation(
            "Fake EmailSender registered in dev mode. " +
            "Email would have been sent to {email}." +
            "\n\nSubject: {EmailSubject}. " +
            "\n\nMessage: {EmailMessage}", 
            email, 
            subject,
            htmlMessage);
        return Task.FromResult(true);
    }

    public Task<bool> SendEmailAsync(string email, string subject, string htmlMessage, string? organizationID = null)
    {
        return SendEmailAsync(email, "", subject, htmlMessage, organizationID);
    }
}