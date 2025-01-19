using UsersMS.Core.Domain.Exceptions;

namespace UsersMS.src.Users.Domain.Exceptions
{
    public class InvalidDeptoNameException : DomainException
    {
        public InvalidDeptoNameException() : base("Invalid department name")
        {
        }
    }
}
