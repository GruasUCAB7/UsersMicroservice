using UsersMS.Core.Domain.Exceptions;

namespace UsersMS.src.Users.Domain.Exceptions
{
    public class InvalidUserPhoneException : DomainException
    {
        public InvalidUserPhoneException() : base("Invalid user phone")
        {
        }
    }
}
