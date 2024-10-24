using UsersMS.Core.Utils.Optional;
using UsersMS.Core.Utils.Result;

namespace UsersMS.src.Users.Application.Repositories
{
    public interface IUserRepository
    {
        Task<bool> ExistByEmail(string email);
        Task<List<Domain.User>> GetAll(int perPage, int page);
        Task<Optional<Domain.User>> GetById(string id);
        Task<Result<Domain.User>> Save(Domain.User User);
    }
}
