using UsersMS.Core.Application.Services;
using UsersMS.Core.Utils.Result;
using UsersMS.src.Users.Application.Queries.GetById.Types;
using UsersMS.src.Users.Application.Queries.Types;
using UsersMS.src.Users.Application.Repositories;
using UsersMS.src.Users.Application.Exceptions;

namespace UsersMS.src.Users.Application.Queries.GetById
{
    public class GetUserByIdQueryHandler(IUserRepository userRepository) : IService<GetUserByIdQuery, GetUserResponse>
    {
        private readonly IUserRepository _userRepository = userRepository;
        public async Task<Result<GetUserResponse>> Execute(GetUserByIdQuery data)
        {
            var userOptional = await _userRepository.GetById(data.Id);
            Console.WriteLine(userOptional);
            if (!userOptional.HasValue)
            {
                return Result<GetUserResponse>.Failure(new UserNotFoundException());
            }

            var user = userOptional.Unwrap();
            Console.WriteLine(user);
            var response = new GetUserResponse(
                user.GetId(),
                user.GetName(),
                user.GetEmail(),
                user.GetPhone(),
                user.GetUserType(),
                user.GetIsActive(),
                user.GetDepartament()
            );

            return Result<GetUserResponse>.Success(response);
        }
    }
}