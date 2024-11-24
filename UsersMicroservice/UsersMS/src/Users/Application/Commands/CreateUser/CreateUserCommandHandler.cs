using UsersMS.Core.Application.IdGenerator;
using UsersMS.Core.Application.Services;
using UsersMS.Core.Utils.Result;
using UsersMS.src.Users.Application.Commands.CreateUser.Types;
using UsersMS.src.Users.Application.Repositories;
using UsersMS.src.Users.Domain.ValueObjects;
using UsersMS.src.Users.Application.Exceptions;
using UsersMS.src.Users.Domain;
using MongoDB.Driver;

namespace UsersMS.src.Users.Application.Commands.CreateUser
{
    public class CreateUserCommandHandler(
        IUserRepository userRepository, 
        IDeptoRepository deptoRepository,
        IdGenerator<string> idGenerator
    ) : IService<CreateUserCommand, CreateUserResponse>
    {
        private readonly IdGenerator<string> _idGenerator = idGenerator;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IDeptoRepository _deptoRepository = deptoRepository;

        public async Task<Result<CreateUserResponse>> Execute(CreateUserCommand data)
        {
            var isUserExist = await _userRepository.ExistByEmail(data.Email);
            if (isUserExist)
            {
                return Result<CreateUserResponse>.Failure(new UserAlreadyExistException(data.Email));
            }

            var id = _idGenerator.Generate();
            var userType = (UserType)Enum.Parse(typeof(UserType), data.UserType);
            var user = User.CreateUser(
                new UserId(id),
                new UserName(data.Name),
                new UserEmail(data.Email),
                new UserPhone(data.Phone),
                userType,
                new DeptoId(data.Department)
            );
            await _userRepository.Save(user);

            return Result<CreateUserResponse>.Success(new CreateUserResponse(id));
        }
    }
}
