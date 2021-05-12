using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using MimeKit.Text;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Remotely.Server.Services
{
    public interface IEmailSenderEx
    {
        Task<bool> SendEmailAsync(string email, string replyTo, string subject, string htmlMessage, string organizationID = null);
        Task<bool> SendEmailAsync(string email, string subject, string htmlMessage, string organizationID = null);
    }

    public class EmailSenderEx : IEmailSenderEx
    {
        public EmailSenderEx(IApplicationConfig appConfig, IDataService dataService)
        {
            AppConfig = appConfig;
            DataService = dataService;
        }

        private IApplicationConfig AppConfig { get; }
        private IDataService DataService { get; }

        public async Task<bool> SendEmailAsync(string toEmail, string replyTo, string subject, string htmlMessage, string organizationID = null)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(AppConfig.SmtpDisplayName, AppConfig.SmtpEmail));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.ReplyTo.Add(MailboxAddress.Parse(replyTo));
                message.Subject = subject;
                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = htmlMessage
                };

                using var client = new SmtpClient();
                
                if (!string.IsNullOrWhiteSpace(AppConfig.SmtpLocalDomain))
                {
                    client.LocalDomain = AppConfig.SmtpLocalDomain;
                }

                client.CheckCertificateRevocation = AppConfig.SmtpCheckCertificateRevocation;

                await client.ConnectAsync(AppConfig.SmtpHost, AppConfig.SmtpPort);

                if (!string.IsNullOrWhiteSpace(AppConfig.SmtpUserName) &&
                    !string.IsNullOrWhiteSpace(AppConfig.SmtpPassword))
                {
                    await client.AuthenticateAsync(AppConfig.SmtpUserName, AppConfig.SmtpPassword);
                }

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                DataService.WriteEvent($"Email successfully sent to {toEmail}.  Subject: \"{subject}\".", organizationID);

                return true;
            }
            catch (Exception ex)
            {
                DataService.WriteEvent(ex, organizationID);
                return false;
            }
        }

        public Task<bool> SendEmailAsync(string email, string subject, string htmlMessage, string organizationID = null)
        {
            return SendEmailAsync(email, AppConfig.SmtpEmail, subject, htmlMessage, organizationID);
        }
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

}
