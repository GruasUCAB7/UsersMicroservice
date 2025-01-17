using UsersMS.Core.Application.Crypto;

namespace UsersMS.Core.Infrastructure.Crypto
{
    public class BcryptCryptoService : ICrypto
    {
        public Task<string> Encrypt(string value)
        {
            return Task.FromResult(BCrypt.Net.BCrypt.HashPassword(value));
        }

        public bool Compare(string text, string encrypted)
        {
            return BCrypt.Net.BCrypt.Verify(text, encrypted);
        }
    }
}
