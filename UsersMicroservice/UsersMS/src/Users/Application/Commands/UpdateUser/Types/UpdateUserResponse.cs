namespace UsersMS.src.Users.Application.Commands.UpdateUser.Types
{
    public record UpdateUserResponse
    (
        string Id,
        string Name,
        string Email,
        string Phone,
        string UserType,
        string Department,
        bool IsActive
    );
}