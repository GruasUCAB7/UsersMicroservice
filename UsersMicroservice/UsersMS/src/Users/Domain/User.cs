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
        private string _passwordHash;
        private bool _isTemporaryPassword = false;
        private DateTime? _passwordExpirationDate;

        public string GetId() => _id.GetValue();
        public string GetName() => _name.GetValue();
        public string GetEmail() => _email.GetValue();
        public string GetPhone() => _phone.GetValue();
        public string GetUserType() => _userType.GetValue();
        public bool GetIsActive() => _isActive;
        public string GetDepartment() => _department.GetValue();
        public string GetPasswordHash() => _passwordHash;
        public bool GetTemporaryPassword() => _isTemporaryPassword;
        public DateTime? GetPasswordExpirationDate() => _passwordExpirationDate;

        public void SetTemporaryPassword(bool isTemporary) => _isTemporaryPassword = isTemporary;
        public void SetPasswordHash(string passwordHash) => _passwordHash = passwordHash;
        public void SetPasswordExpirationDate(DateTime? expirationDate) => _passwordExpirationDate = expirationDate;
        public void SetPhone(string phone) => _phone = new UserPhone(phone);
        public void SetUserType(string userType) => _userType = new UserType(userType);
        public void SetIsActive(bool isActive) => _isActive = isActive;
        public void SetDepartment(string depto) => _department = new DeptoName(depto);

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
            if (_id == null || _name == null || _email == null || _phone == null || _userType == null || _department == null)
            {
                throw new InvalidUserException();
            }
        }
    }
}
