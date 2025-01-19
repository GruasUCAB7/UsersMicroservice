namespace UsersMS.Core.Domain.Services
{
    public interface IDomainService<T, R>
    {
        R Execute(T data);
    }
}
