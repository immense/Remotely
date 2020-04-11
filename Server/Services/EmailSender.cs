using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Remotely.Server.Services
{
    public interface IEmailSenderEx
    {
        Task<bool> SendEmailAsync(string email, string replyTo, string subject, string htmlMessage, string organizationID = null);
        Task<bool> SendEmailAsync(string email, string subject, string htmlMessage, string organizationID = null);
    }

    public class EmailSenderEx : IEmailSenderEx
    {
        public EmailSenderEx(ApplicationConfig appConfig, DataService dataService)
        {
            AppConfig = appConfig;
            DataService = dataService;
        }

        private ApplicationConfig AppConfig { get; }
        private DataService DataService { get; }

        public Task<bool> SendEmailAsync(string email, string replyTo, string subject, string htmlMessage, string organizationID = null)
        {
            try
            {
                var mailClient = new SmtpClient();
                mailClient.Host = AppConfig.SmtpHost;
                mailClient.Port = AppConfig.SmtpPort;
                mailClient.EnableSsl = AppConfig.SmtpEnableSsl;
                mailClient.Credentials = new NetworkCredential(AppConfig.SmtpUserName, AppConfig.SmtpPassword);
                mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                var from = new MailAddress(AppConfig.SmtpEmail, AppConfig.SmtpDisplayName, System.Text.Encoding.UTF8);
                var to = new MailAddress(email);

                var mailMessage = new MailMessage(from, to);
                mailMessage.IsBodyHtml = true;
                mailMessage.Subject = subject;
                mailMessage.Body = htmlMessage;
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
