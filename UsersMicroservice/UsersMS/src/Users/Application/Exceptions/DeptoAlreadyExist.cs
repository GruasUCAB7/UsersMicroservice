namespace UsersMS.src.Users.Application.Exceptions
{
    public class DeptoAlreadyExistException : ApplicationException
    {
        public DeptoAlreadyExistException(string name)
            : base($"Department with name {name} already exists")
        {
        }
    }
}
