namespace UsersMS.src.Users.Application.Queries.Types
{
    public record GetUserResponse
    (
        string Id,
        string Name,
        string Email,
        string Phone,
        string UserType,
        bool IsActive,
        string Department
    );
}
