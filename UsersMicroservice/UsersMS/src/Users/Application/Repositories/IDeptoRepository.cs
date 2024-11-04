using UsersMS.Core.Utils.Optional;
using UsersMS.Core.Utils.Result;
using UsersMS.src.Users.Domain.Entities;

namespace UsersMS.src.Users.Application.Repositories
{
    public interface IDeptoRepository
    {
        Task<bool> ExistByName(string name);
        Task<List<Department>> GetAll(int perPage, int page);
        Task<Optional<Department>> GetById(string id);
        Task<Result<Department>> Save(Department Depto);    }
}
