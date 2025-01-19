using DotNetEnv;
using System.Net;
using System.Net.Mail;
using UsersMS.Core.Application.EmailSender;
using UsersMS.Core.Application.EmailSender.Types;

namespace UsersMS.Core.Infrastructure.EmailSender
{
    public class EmailService : IEmailSender
    {
        public async Task SendEmail(EmailSenderDto data)
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
                    Subject = data.Subject,
                    Body = data.Body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(data.RecipientEmail);

                await smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
}
