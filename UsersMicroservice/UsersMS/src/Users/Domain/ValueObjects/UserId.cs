using UsersMS.Core.Domain.ValueObjects;
using UsersMS.Core.Utils.RegExps;
using UsersMS.src.Users.Domain.Exceptions;

namespace UsersMS.src.Users.Domain.ValueObjects
{
    public class UserId : IValueObject<UserId>
    {
        private string Id { get; }

        public UserId(string id)
        {
            if (!UUIDRegExps.UUIDRegExp.IsMatch(id))
            {
                throw new InvalidUserIdException();
            }
            Id = id;
        }

        public string GetValue()
        {
            return Id;
        }

        public bool Equals(UserId other)
        {
            return Id == other.Id;
        }
    }
}
