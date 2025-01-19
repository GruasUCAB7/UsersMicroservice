namespace UsersMS.src.Users.Application.Commands.CreateUser.Types
{
    public record CreateUserClientCommand(
        string Name,
        string Email,
        string Phone,
        string UserType,
        string Department
    );
}
