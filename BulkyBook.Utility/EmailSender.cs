using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;

namespace BulkyBook.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var emailMessage = new MimeMessage();
            var fromName = _configuration["EmailSettings:FromName"];
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            emailMessage.From.Add(new MailboxAddress(fromName, fromEmail));
            emailMessage.To.Add(new MailboxAddress(toEmail, toEmail));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = htmlMessage
            };

            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_configuration["EmailSettings:MailSmtpServer"], 587, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_configuration["EmailSettings:MailServerUserName"], _configuration["EmailSettings:MailServerPassword"]);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
