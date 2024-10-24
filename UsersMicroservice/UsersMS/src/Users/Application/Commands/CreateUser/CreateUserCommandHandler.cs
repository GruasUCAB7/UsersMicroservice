using UsersMS.Core.Application.IdGenerator;
using UsersMS.Core.Application.Services;
using UsersMS.Core.Utils.Result;
using UsersMS.src.Users.Application.Commands.CreateUser.Types;
using UsersMS.src.Users.Application.Repositories;
using UsersMS.src.Users.Domain.ValueObjects;
using UsersMS.src.Users.Application.Exceptions;
using UsersMS.src.Users.Domain;

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

        public async Task<Result<CreateUserResponse>> Execute(CreateUserCommand command)
        {
            var isUserExist = await _userRepository.ExistByEmail(command.Email);
            if (isUserExist)
            {
                return Result<CreateUserResponse>.Failure(new UserAlreadyExistException(command.Email));
            }

            var id = _idGenerator.Generate();

            var userType = (UserType)Enum.Parse(typeof(UserType), command.UserType);
            if (userType == UserType.Admin)
            {
                return Result<CreateUserResponse>.Failure(new InvalidUserTypeAdmin());
            }

            var status = true;
            var depto = await _deptoRepository.GetById(command.Department);

            var user = new User(
                new UserId(id),
                new UserName(command.Name),
                new UserEmail(command.Email),
                new UserPhone(command.Phone),
                userType,
                status,
                new DeptoId(command.Department)
            );

            await _userRepository.Save(user);

            return Result<CreateUserResponse>.Success(new CreateUserResponse(id));
        }
    }
}
