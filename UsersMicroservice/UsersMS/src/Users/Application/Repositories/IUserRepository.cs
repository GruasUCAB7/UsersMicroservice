using UsersMS.Core.Utils.Optional;
using UsersMS.Core.Utils.Result;
using UsersMS.src.Users.Application.Queries.GetAllUsers.Types;
using UsersMS.src.Users.Domain;

namespace UsersMS.src.Users.Application.Repositories
{
    public interface IUserRepository
    {
        Task<bool> ExistByEmail(string email);
        Task<Result<User>> Save(User User);
        Task<List<User>> GetAll(GetAllUsersQuery data);
        Task<Optional<User>> GetById(string id);
        Task<Result<User>> Update(User user);
    }
}
