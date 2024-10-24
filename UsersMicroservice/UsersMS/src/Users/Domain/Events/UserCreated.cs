using UsersMS.Core.Domain.Events;
using UsersMS.src.Users.Domain.ValueObjects;

namespace UsersMS.src.Users.Domain.Events
{
    public class UserCreated(string id, string name, string email, string phone, string userType, bool status, string department)
    {
        public readonly string Id = id;
        public readonly string Name = name;
        public readonly string Email = email;
        public readonly string Phone = phone;
        public readonly string UserType = userType;
        public readonly bool Status = status = true;
        public readonly string Department = department;

    }
    public class UserCreatedEvent(string dispatcherId, string name, UserCreated context) : DomainEvent<object>(dispatcherId, name, context)
    {

        public static UserCreatedEvent CreateEvent(UserId UserId, UserName UserName, UserEmail UserEmail, UserPhone UserPhone, UserType UserType, bool Status, DeptoId department)
        {
            return new UserCreatedEvent(
                UserId.GetValue(),
                typeof(UserCreated).Name,
                new UserCreated(
                    UserId.GetValue(),
                    UserName.GetValue(),
                    UserEmail.GetValue(),
                    UserPhone.GetValue(),
                    UserType.GetValue(),
                    Status,
                    department.GetValue()
                )
            );
        }
    }
}
