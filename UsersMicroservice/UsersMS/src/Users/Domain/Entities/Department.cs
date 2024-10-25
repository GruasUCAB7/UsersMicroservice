using UsersMS.Core.Domain.Entities;
using UsersMS.src.Users.Domain.ValueObjects;

namespace UsersMS.src.Users.Domain.Entities
{
    public class Department : Entity<DeptoId>
    {
        private new DeptoId _id;
        private DeptoName _name;
        private DeptoDescription _description;
        private bool _status = true;

        public Department(DeptoId id, DeptoName name, DeptoDescription description, bool status) : base(id)
        {
            _id = id;
            _name = name;
            _description = description;
            _status = status;
        }

        public DeptoId GetId()
        {
            return _id;
        }

        public DeptoName GetName()
        {
            return _name;
        }

        public DeptoDescription GetDescription()
        {
            return _description;
        }

        public bool GetStatus()
        {
            return _status;
        }

        public void ChangeName(DeptoName name)
        {
            this._name = name;
        }

        public void ChangeDescription(DeptoDescription description)
        {
            this._description = description;
        }

        public void ChangeStatus(bool status)
        {
            this._status = status;
        }
    }
}
