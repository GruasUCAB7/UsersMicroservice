using DotNetEnv;
using System.Net;
using System.Net.Mail;

namespace UsersMS.src.Users.Infrastructure.Services
{
    public class EmailService(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;

        public async Task SendEmail(string recipientEmail, string subject, string body)
        {
            var smtpHost = Env.GetString("EMAIL_SMTP_HOST");
            var smtpPort = int.Parse(Env.GetString("EMAIL_SMTP_PORT"));
            var senderEmail = Env.GetString("EMAIL_SENDER_EMAIL");
            var senderPassword = Env.GetString("EMAIL_SENDER_PASSWORD");
            var senderName = Env.GetString("EMAIL_SENDER_NAME");

            using (var smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true
            })
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(recipientEmail);

                await smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
}
