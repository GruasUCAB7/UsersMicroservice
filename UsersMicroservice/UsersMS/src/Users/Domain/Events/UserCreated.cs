using UsersMS.Core.Domain.Events;
using UsersMS.src.Users.Domain.ValueObjects;

namespace UsersMS.src.Users.Domain.Events
{
    public class UserCreatedEvent(string dispatcherId, string name, UserCreated context) : DomainEvent<object>(dispatcherId, name, context){ }
    public class UserCreated(string id, string name, string email, string phone, string userType, string department)
    {
        public readonly string Id = id;
        public readonly string Name = name;
        public readonly string Email = email;
        public readonly string Phone = phone;
        public readonly string UserType = userType;
        public readonly string Department = department;

        public static UserCreatedEvent CreateEvent(UserId userId, UserName userName, UserEmail userEmail, UserPhone userPhone, UserType userType, DeptoId department)
        {
            return new UserCreatedEvent(
                userId.GetValue(),
                typeof(UserCreated).Name,
                new UserCreated(
                    userId.GetValue(),
                    userName.GetValue(),
                    userEmail.GetValue(),
                    userPhone.GetValue(),
                    userType.GetValue(),
                    department.GetValue()
                )
            );
        }

    }
}
