using UsersMS.Core.Domain.Exceptions;

namespace UsersMS.src.Users.Domain.Exceptions
{
    public class InvalidUserException : DomainException
    {
        public InvalidUserException() : base("Invalid user")
        {
        }
    }
}
