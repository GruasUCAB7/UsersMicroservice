using UsersMS.src.Users.Domain.Exceptions;

namespace UsersMS.src.Users.Domain.ValueObjects
{
    public static class UserTypeValue
    {
        public static string GetValue(this UserType userType)
        {
            if (userType == UserType.Operator || userType == UserType.Driver || userType == UserType.Provider)
            {
                return userType.ToString();
            }
            throw new InvalidUserTypeException();
        }
    }
}