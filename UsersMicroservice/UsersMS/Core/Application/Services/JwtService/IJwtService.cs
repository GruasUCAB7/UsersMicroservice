namespace UsersMS.Core.Application.Services.JwtService
{
    public interface IJwtService
    {
        string GenerateToken(string userId, string name, string email, string phone, bool isTemporaryPassword, string role);
        string GenerateLimitedToken(string userId, string email);
    }
}
