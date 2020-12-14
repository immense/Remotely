using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Net;
using System.Net.Mail;
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

        public Task<bool> SendEmailAsync(string email, string replyTo, string subject, string htmlMessage, string organizationID = null)
        {
            try
            {
                var mailClient = new SmtpClient
                {
                    Host = AppConfig.SmtpHost,
                    Port = AppConfig.SmtpPort,
                    EnableSsl = AppConfig.SmtpEnableSsl,
                    Credentials = new NetworkCredential(AppConfig.SmtpUserName, AppConfig.SmtpPassword),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                var from = new MailAddress(AppConfig.SmtpEmail, AppConfig.SmtpDisplayName, System.Text.Encoding.UTF8);
                var to = new MailAddress(email);

                var mailMessage = new MailMessage(from, to)
                {
                    IsBodyHtml = true,
                    Subject = subject,
                    Body = htmlMessage
                };
                mailMessage.ReplyToList.Add(new MailAddress(replyTo));
                mailClient.Send(mailMessage);
                DataService.WriteEvent($"Email successfully sent to {email}.  Subject: \"{subject}\".", organizationID);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                DataService.WriteEvent(ex, organizationID);
                return Task.FromResult(false);
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
