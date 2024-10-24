using UsersMS.Core.Domain.ValueObjects;
using UsersMS.Core.Utils.RegExps;
using UsersMS.src.Users.Domain.Exceptions;

namespace UsersMS.src.Users.Domain.ValueObjects
{
    public class DeptoId : IValueObject<DeptoId>
    {
        private string Id { get; }

        public DeptoId(string id)
        {
            if (!UUIDRegExps.UUIDRegExp.IsMatch(id))
            {
                throw new InvalidDeptoIdException();
            }
            Id = id;
        }

        public string GetValue()
        {
            return Id;
        }

        public bool Equals(DeptoId other)
        {
            return Id == other.Id;
        }
    }
}
