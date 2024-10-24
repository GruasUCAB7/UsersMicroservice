using UsersMS.Core.Domain.ValueObjects;
using UsersMS.src.Users.Domain.Exceptions;

namespace UsersMS.src.Users.Domain.ValueObjects
{
    public class UserName : IValueObject<UserName>
    {
        private string Name { get; }

        public UserName(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
            {
                throw new InvalidUserNameException();
            }
            Name = name;
        }
        
        public string GetValue()
        {
            return Name;
        }

        public bool Equals(UserName other)
        {
            return Name == other.Name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

