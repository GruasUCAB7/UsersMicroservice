namespace UsersMS.src.Users.Application.Commands.UpdateUser.Types
{
    public record UpdateUserCommand
    (
        bool? IsActive,
        string? Phone,
        string? Department,
        string? UserType,
        bool? IsTemporaryPassword,
        DateTime? PasswordExpirationDate
    );
}