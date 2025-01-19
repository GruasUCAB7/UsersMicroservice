using UsersMS.Core.Domain.Exceptions;

namespace UsersMS.src.Users.Domain.Exceptions
{
    public class InvalidUserTypeException(string message) : DomainException(message)
    {
    }
}
