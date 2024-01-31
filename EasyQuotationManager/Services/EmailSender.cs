using EasyQuotationManager.Shared.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace EasyQuotationManager.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        public EmailOptions _emailOptions;
        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
            _emailOptions = _configuration.GetSection("EmailSender").Get<EmailOptions>();
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient(_emailOptions.Host, _emailOptions.Port)
            {
                Credentials = new NetworkCredential(_emailOptions.UserName, _emailOptions.Password),
                EnableSsl = _emailOptions.EnableSSL,
                UseDefaultCredentials = false
            };

            try
            {
                return client.SendMailAsync(new MailMessage(_emailOptions.MailFrom, email, subject, htmlMessage) { IsBodyHtml = true });
            }
            catch (SmtpFailedRecipientsException)
            {
                throw;
            }
        }

        public void SendEmail(string email, string subject, string htmlMessage)
        {
            try
            {
                using (MailMessage mm = new MailMessage(_emailOptions.MailFrom, email))
                {
                    mm.Subject = subject;
                    mm.Body = htmlMessage;
                    mm.IsBodyHtml = true;

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = _emailOptions.Host;
                    smtp.EnableSsl = _emailOptions.EnableSSL;
                    smtp.UseDefaultCredentials = false;

                    NetworkCredential credential = new NetworkCredential(_emailOptions.UserName, _emailOptions.Password);
                    smtp.Credentials = credential;
                    smtp.Port = _emailOptions.Port;
                    smtp.Send(mm);
                    mm.Dispose();
                }
            }
            catch (SmtpFailedRecipientsException)
            {
                throw;
            }
        }

        public void SendEmailWithMultipleRecipient(string email, string subject, string htmlMessage)
        {
            try
            {
                var mailMessage = new MailMessage();
                mailMessage.To.Add(new MailAddress(email));
                mailMessage.Bcc.Add("test@test.com"); // If we need any BCC, we can add here that email address

                mailMessage.From = new MailAddress(_emailOptions.MailFrom);
                mailMessage.Subject = subject;
                mailMessage.Body = htmlMessage;
                mailMessage.IsBodyHtml = true;

                foreach (var address in email.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    mailMessage.To.Add(address);
                }

                using (var smtp = new SmtpClient())
                {
                    smtp.UseDefaultCredentials = false;
                    var credential = new NetworkCredential { UserName = _emailOptions.UserName, Password = _emailOptions.Password };
                    smtp.Credentials = credential;
                    smtp.Host = _emailOptions.Host;
                    smtp.Port = _emailOptions.Port;
                    smtp.EnableSsl = _emailOptions.EnableSSL;
                    ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                    smtp.Send(mailMessage);
                    mailMessage.Dispose();
                }
            }
            catch (SmtpFailedRecipientsException)
            {
                throw;
            }
        }

    }
}
