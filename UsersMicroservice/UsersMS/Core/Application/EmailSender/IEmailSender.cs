using UsersMS.Core.Application.EmailSender.Types;

namespace UsersMS.Core.Application.EmailSender
{
    public interface IEmailSender
    {
        Task SendEmail(EmailSenderDto data);
    }
}
