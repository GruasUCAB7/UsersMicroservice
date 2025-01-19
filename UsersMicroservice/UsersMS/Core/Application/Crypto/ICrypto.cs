namespace UsersMS.Core.Application.Crypto
{
    public interface ICrypto
    {
        Task<string> Encrypt(string value);

        bool Compare(string text, string encrypted);
    }
}