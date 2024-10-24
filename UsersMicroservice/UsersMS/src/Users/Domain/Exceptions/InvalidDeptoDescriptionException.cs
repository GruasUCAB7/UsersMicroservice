using UsersMS.Core.Domain.Exceptions;

namespace UsersMS.src.Users.Domain.Exceptions
{
    public class InvalidDeptoDescriptionException : DomainException
    {
        public InvalidDeptoDescriptionException() : base("Invalid department description")
        {
        }
    }
}
