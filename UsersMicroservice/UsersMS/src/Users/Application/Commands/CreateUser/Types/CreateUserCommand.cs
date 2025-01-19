namespace UsersMS.src.Users.Application.Commands.CreateUser.Types
{
    public record CreateUserCommand(
        string Name,
        string Email,
        string Phone,
        string UserType,
        string Department,
        string PasswordHash,
        bool IsTemporaryPassword,
        DateTime PasswordExpirationDate
    );
}
