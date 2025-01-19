namespace UsersMS.src.Users.Infrastructure.Types
{
    public class UserChangePasswordRequest
    {
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

}
