namespace UsersMS.src.Users.Application.Commands.UpdateUser.Types
{
    public record UpdateUserResponse
    (
        string Id,
        string Name,
        string Email,
        string phone,
        string userType,
        string department,
        bool IsActive
    );
}