namespace UsersMS.src.Users.Application.Exceptions
{
    public class UserAlreadyExistException(string email) : ApplicationException($"User with email {email} already exists")
    {
    }
}
