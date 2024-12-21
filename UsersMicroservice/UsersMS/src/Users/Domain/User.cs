using UsersMS.Core.Domain.Aggregates;
using UsersMS.src.Users.Domain.Events;
using UsersMS.src.Users.Domain.Exceptions;
using UsersMS.src.Users.Domain.ValueObjects;

namespace UsersMS.src.Users.Domain
{
    public class User(UserId id) : AggregateRoot<UserId>(id)
    {
        private UserId _id = id;
        private UserName _name;
        private UserEmail _email;
        private UserPhone _phone;
        private UserType _userType;
        private DeptoName _department;
        private bool _isActive = true;

        public string GetId() => _id.GetValue();
        public string GetName() => _name.GetValue();
        public string GetEmail() => _email.GetValue();
        public string GetPhone() => _phone.GetValue();
        public string GetUserType() => _userType.GetValue();
        public bool GetIsActive() => _isActive;
        public string GetDepartment() => _department.GetValue();
        public void SetPhone(string phone) => _phone = new UserPhone(phone);
        public bool SetIsActive(bool isActive) => _isActive = isActive;
        public void SetDepartment(string depto) => _department = new DeptoName(depto);
        public void SetUserType(string type) => _userType = new UserType(type);

        public static User CreateUser(UserId id, UserName name, UserEmail email, UserPhone phone, UserType userType, DeptoName department)
        {
            var user = new User(id);
            user.Apply(UserCreated.CreateEvent(id, name, email, phone, userType, department));
            return user;
        }

        public void OnUserCreatedEvent(UserCreated context)
        {
            _id = new UserId(context.Id);
            _name = new UserName(context.Name);
            _email = new UserEmail(context.Email);
            _phone = new UserPhone(context.Phone);
            _userType = new UserType(context.UserType);
            _department = new DeptoName(context.Department);
        }

        public override void ValidateState()
        {
            if (_id == null || _name == null || _email == null || _phone == null || _department == null )
            {
                throw new InvalidUserException();
            }
        }
    }
}
