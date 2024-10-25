using UsersMS.Core.Application.IdGenerator;

namespace UsersMS.src.Users.Infrastructure.UUID
{
    public class GuidGenerator : IdGenerator<string>
    {
        public string Generate()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
