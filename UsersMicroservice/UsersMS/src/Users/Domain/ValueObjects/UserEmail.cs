using UsersMS.Core.Domain.ValueObjects;
using UsersMS.src.Users.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace UsersMS.src.Users.Domain.ValueObjects
{
    public class UserEmail : IValueObject<UserEmail>
    {
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        private string Email { get; }

        public UserEmail(string email)
        {
            if (!EmailRegex.IsMatch(email))
            {
                throw new InvalidUserEmailException();
            }
            Email = email;
        }

        public string GetValue()
        {
            return Email;
        }

        public bool Equals(UserEmail other)
        {
            return Email == other.Email;
        }

        public override string ToString()
        {
            return Email;
        }
    }
}