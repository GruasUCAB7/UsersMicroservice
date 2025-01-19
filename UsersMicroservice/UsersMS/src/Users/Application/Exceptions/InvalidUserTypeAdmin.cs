namespace UsersMS.src.Users.Application.Exceptions
{
    public class InvalidUserTypeAdmin : ApplicationException
    {
        public InvalidUserTypeAdmin()
            : base("Creating users with UserType 'Admin' is not allowed.")
        {
        }
    }
}
