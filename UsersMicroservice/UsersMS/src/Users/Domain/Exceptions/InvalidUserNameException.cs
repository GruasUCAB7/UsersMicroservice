using UsersMS.Core.Domain.Exceptions;

namespace UsersMS.src.Users.Domain.Exceptions
{
    public class InvalidUserNameException : DomainException
    {
        public InvalidUserNameException() : base("Invalid user name")
        {
        }
    }
}
