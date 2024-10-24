namespace UsersMS.src.Users.Application.Exceptions
{
    public class UserAlreadyExistException : ApplicationException
    {
        public UserAlreadyExistException(string email)
            : base($"User with email {email} already exists")
        {
        }
    }
}
