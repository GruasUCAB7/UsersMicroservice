namespace UsersMS.src.Users.Application.Commands.AddNotificationToken.Types
{
    public record AddNotificationTokenCommand(
        string UserId,
        string Token
    );
}
