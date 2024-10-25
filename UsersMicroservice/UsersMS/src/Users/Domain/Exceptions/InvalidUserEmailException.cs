using UsersMS.Core.Domain.Exceptions;

namespace UsersMS.src.Users.Domain.Exceptions
{
    public class InvalidUserEmailException : DomainException
    {
        public InvalidUserEmailException() : base("Invalid user email")
        {
        }
    }
}
