namespace UsersMS.Core.Application.EmailSender.Types
{
    public record EmailSenderDto(
        string RecipientEmail,
        string Subject,
        string Body
    );
}
