using System;

namespace UsersMS.src.Users.Application.Exceptions
{
    public class UserUpdateFailedException : Exception
    {
        public UserUpdateFailedException(string message) : base(message) { }
    }
}