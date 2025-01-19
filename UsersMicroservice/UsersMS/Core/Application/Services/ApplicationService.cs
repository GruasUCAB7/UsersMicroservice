using UsersMS.Core.Utils.Result;

namespace UsersMS.Core.Application.Services
{
    public interface IService<T, R>
    {
        Task<Result<R>> Execute(T data);
    }
}
