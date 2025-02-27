using UsersMS.Core.Application.Services;
using UsersMS.Core.Utils.Result;
using UsersMS.src.Users.Application.Commands.UpdateUser.Types;
using UsersMS.src.Users.Application.Repositories;
using UsersMS.src.Users.Application.Exceptions;

namespace UsersMS.src.Users.Application.Commands.UpdateUser
{
    public class UpdateUserCommandHandler(IUserRepository userRepository) : IService<(string id, UpdateUserCommand data), UpdateUserResponse>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<UpdateUserResponse>> Execute((string id, UpdateUserCommand data) request)
        {
            var userOptional = await _userRepository.GetById(request.id);
            if (!userOptional.HasValue)
            {
                return Result<UpdateUserResponse>.Failure(new UserNotFoundException());
            }

            var user = userOptional.Unwrap();

            if (request.data.IsActive.HasValue)
            {
                user.SetIsActive(request.data.IsActive.Value);
            }

            if (!string.IsNullOrEmpty(request.data.Phone))
            {
                user.SetPhone(request.data.Phone);
            }

            if (!string.IsNullOrEmpty(request.data.Department))
            {
                user.SetDepartment(request.data.Department);
            }

            if (!string.IsNullOrEmpty(request.data.UserType))
            {
                user.SetUserType(request.data.UserType);
            }

            if (request.data.IsTemporaryPassword.HasValue)
            {
                user.SetTemporaryPassword(request.data.IsTemporaryPassword.Value);
            }

            if (request.data.PasswordExpirationDate.HasValue)
            {
                user.SetPasswordExpirationDate(request.data.PasswordExpirationDate);
            }

            var updateResult = await _userRepository.Update(user);
            if (updateResult.IsFailure)
            {
                return Result<UpdateUserResponse>.Failure(new UserUpdateFailedException());
            }

            var response = new UpdateUserResponse(
                user.GetId(),
                user.GetName(),
                user.GetEmail(),
                user.GetPhone(),
                user.GetUserType(),
                user.GetDepartment(),
                user.GetIsActive(),
                user.GetTemporaryPassword(),
                user.GetPasswordExpirationDate()
            );

            return Result<UpdateUserResponse>.Success(response);
        }
    }
}
