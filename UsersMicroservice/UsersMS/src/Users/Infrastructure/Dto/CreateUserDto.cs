namespace UsersMS.src.Users.Infrastructure.Dto
{
    public record CreateUserDto
    (
        string Name,
        string Email,
        string Phone,
        string UserType,
        string Departament
    );
}
