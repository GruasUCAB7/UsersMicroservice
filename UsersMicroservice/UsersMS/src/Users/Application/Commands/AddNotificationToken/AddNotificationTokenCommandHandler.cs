using UsersMS.Core.Application.IdGenerator;
using UsersMS.Core.Application.Services;
using UsersMS.Core.Utils.Result;
using UsersMS.src.Users.Application.Commands.AddNotificationToken.Types;
using UsersMS.src.Users.Application.Exceptions;
using UsersMS.src.Users.Application.Models;
using UsersMS.src.Users.Application.Repositories;

namespace UsersMS.src.Users.Application.Commands.AddNotificationToken
{
    public class AddNotificationTokenCommandHandler(
        IUserRepository userRepository,
        IdGenerator<string> idGenerator
    ) : IService<AddNotificationTokenCommand, AddNotificationTokenResponse>
    {
        private readonly IdGenerator<string> _idGenerator = idGenerator;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<AddNotificationTokenResponse>> Execute(AddNotificationTokenCommand data)
        {
            var isTokenExist = await _userRepository.ExistTokenByUserId(data.UserId);
            if (isTokenExist)
            {
                return Result<AddNotificationTokenResponse>.Failure(new UserAlreadyHasTokenAssignedException(data.UserId));
            }

            var token = new NotificationToken
            {
                Id = _idGenerator.Generate(),
                UserId = data.UserId,
                Token = data.Token
            };

            await _userRepository.SaveToken(token);

            return Result<AddNotificationTokenResponse>.Success(new AddNotificationTokenResponse(token.Id));
        }

    }
}

