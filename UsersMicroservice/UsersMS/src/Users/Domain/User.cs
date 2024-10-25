using System.Numerics;
using System.Xml.Linq;
using UsersMS.Core.Domain.Aggregates;
using UsersMS.Core.Utils.RegExps;
using UsersMS.src.Users.Domain.Entities;
using UsersMS.src.Users.Domain.Events;
using UsersMS.src.Users.Domain.Exceptions;
using UsersMS.src.Users.Domain.ValueObjects;

namespace UsersMS.src.Users.Domain
{
    public class User : AggregateRoot<UserId>
    {
        private new UserId _id;
        private UserName _name;
        private UserEmail _email;
        private UserPhone _phone;
        private UserType _userType;
        private DeptoId _department;
        private bool _status = true;

        public User(UserId id, UserName name, UserEmail email, UserPhone phone, UserType userType, bool status, DeptoId department) : base(id)
        {
            _id = id;
            _name = name;
            _email = email;
            _phone = phone;
            _userType = userType;
            _status = status;
            _department = department;
            ValidateState();
        }
        

        public UserId GetId()
        {
            return _id;
        }

        public UserName GetName()
        {
            return _name;
        }

        public UserEmail GetEmail()
        {
            return _email;
        }

        public UserPhone GetPhone()
        {
            return _phone;
        }
        
        public UserType GetUserType()
        {
            return _userType;
        }

        public bool GetStatus()
        {
            return _status;
        }

        public DeptoId GetDepartament()
        {
            return _department;
        }

        public User CreateUser(UserId id, UserName name, UserEmail email, UserPhone phone, UserType userType, bool status, DeptoId department)
        {
            var user = new User(id, name, email, phone, userType, status, department);
            user.Apply(UserCreatedEvent.CreateEvent(id, name, email, phone, userType, status, department));
            return user;
        }

        public void OnUserCreatedEvent(UserCreated context)
        {
            _id = new UserId(context.Id);
            _name = new UserName(context.Name);
            _email = new UserEmail(context.Email);
            _phone = new UserPhone(context.Phone);
            _userType = Enum.Parse<UserType>(context.UserType);
            _status = context.Status;
            _department = new DeptoId(context.Department);
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
