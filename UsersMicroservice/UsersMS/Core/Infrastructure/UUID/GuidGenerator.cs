using UsersMS.Core.Application.IdGenerator;

namespace UsersMS.Core.Infrastructure.UUID
{
    public class GuidGenerator : IdGenerator<string>
    {
        public string Generate()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
