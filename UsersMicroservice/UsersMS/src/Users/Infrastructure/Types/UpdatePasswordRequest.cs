namespace UsersMS.src.Users.Infrastructure.Types
{
    public class UpdatePasswordRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}