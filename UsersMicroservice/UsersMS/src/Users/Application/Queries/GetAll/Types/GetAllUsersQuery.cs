namespace UsersMS.src.Users.Application.Queries.GetAllUsers.Types
{
    public record GetAllUsersQuery(
        int PerPage,
        int Page,
        string? IsActive
        );
}
