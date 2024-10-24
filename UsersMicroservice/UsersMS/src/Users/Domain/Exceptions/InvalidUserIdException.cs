using UsersMS.Core.Domain.Exceptions;

namespace UsersMS.src.Users.Domain.Exceptions
{
    public class InvalidUserIdException : DomainException
    {
        public InvalidUserIdException() : base("Invalid user ID")
        {
        }
    }
}
