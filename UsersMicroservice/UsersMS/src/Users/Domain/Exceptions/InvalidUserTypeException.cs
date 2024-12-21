using UsersMS.Core.Domain.Exceptions;

namespace UsersMS.src.Users.Domain.Exceptions
{
    public class InvalidUserTypeException : DomainException
    {
        public InvalidUserTypeException() : base("Invalid user type. Options: Operator, Provider, Driver")
        {
        }
    }
}
