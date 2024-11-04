using UsersMS.Core.Domain.Events;
using UsersMS.src.Users.Domain.ValueObjects;

namespace UsersMS.src.Users.Domain.Events
{
    public class UserUpdatedEvent(string id, string name, UserUpdated context) : DomainEvent<object>(id, name, context) { }

    public class UserUpdated(string id, bool? isActive, string? phone)
    {
        public readonly string Id = id;
        public readonly bool? IsActive = isActive;
        public readonly string? Phone = phone;

        static public UserUpdatedEvent CreateEvent(UserId userId, bool? isActive, string? phone)
        {
            return new UserUpdatedEvent(
                userId.GetValue(),
                typeof(UserUpdated).Name,
                new UserUpdated(
                    userId.GetValue(),
                    isActive,
                    phone
                )
            );
        }
    }
}