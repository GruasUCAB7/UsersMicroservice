namespace UsersMS.src.Users.Application.Exceptions
{
    public class UserAlreadyHasTokenAssignedException(string id) : ApplicationException($"This user whit id: {id} already has a token assigned.")
    {
    }
}
