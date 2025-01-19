using UsersMS.Core.Domain.ValueObjects;
using UsersMS.src.Users.Domain.Exceptions;

namespace UsersMS.src.Users.Domain.ValueObjects
{
    public class DeptoName : IValueObject<DeptoName>
    {
        private string Name { get; }

        public DeptoName(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length < 1)
            {
                throw new InvalidDeptoNameException();
            }
            Name = name;
        }
        
        public string GetValue()
        {
            return Name;
        }

        public bool Equals(DeptoName other)
        {
            return Name == other.Name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

