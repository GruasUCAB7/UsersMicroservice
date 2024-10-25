using UsersMS.Core.Domain.ValueObjects;
using UsersMS.src.Users.Domain.Exceptions;

namespace UsersMS.src.Users.Domain.ValueObjects
{
    public class DeptoDescription : IValueObject<DeptoDescription>
    {
        private string Description { get; }

        public DeptoDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description) || description.Length < 5)
            {
                throw new InvalidDeptoDescriptionException();
            }
            Description = description;
        }
        
        public string GetValue()
        {
            return Description;
        }

        public bool Equals(DeptoDescription other)
        {
            return Description == other.Description;
        }

        public override string ToString()
        {
            return Description;
        }
    }
}

