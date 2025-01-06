namespace UsersMS.src.Users.Infrastructure.Types
{
    public class UserChangePasswordRequest
    {
        public required string Email { get; set; }
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
