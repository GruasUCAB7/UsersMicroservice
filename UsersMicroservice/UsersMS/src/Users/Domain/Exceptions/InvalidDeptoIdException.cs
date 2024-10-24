using UsersMS.Core.Domain.Exceptions;

namespace UsersMS.src.Users.Domain.Exceptions
{
    public class InvalidDeptoIdException : DomainException
    {
        public InvalidDeptoIdException() : base("Invalid department ID")
        {
        }
    }
}
