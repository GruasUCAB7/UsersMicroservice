using UsersMS.Core.Domain.ValueObjects;
using UsersMS.src.Users.Domain.Exceptions;
using UsersMS.Core.Utils.RegExps;
using System.Text.RegularExpressions;

namespace UsersMS.src.Users.Domain.ValueObjects
{
    public class UserPhone : IValueObject<UserPhone>
    {
        private string Phone { get; }

        public UserPhone(string phone)
        {
            if (!PhoneNumberRegex.IsMatch(phone))
            {
                throw new InvalidUserPhoneException();
            }
            Phone = phone;
        }

        public string GetValue()
        {
            return this.Phone;
        }

        public bool Equals(UserPhone other)
        {
            return Phone == other.Phone;
        }

        public override string ToString()
        {
            return Phone;
        }
    }
}
