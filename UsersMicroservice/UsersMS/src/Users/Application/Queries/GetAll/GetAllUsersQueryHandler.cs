using System.Collections.Generic;
using System.Threading.Tasks;
using UsersMS.Core.Utils.Result;
using UsersMS.src.Users.Application.Repositories;
using UsersMS.Core.Application.Services;
using UsersMS.src.Users.Application.Queries.GetAllUsers.Types;
using UsersMS.src.Users.Application.Queries.Types;
using UsersMS.src.Users.Application.Exceptions;

namespace UsersMS.src.Users.Application.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler(IUserRepository userRepository) : IService<GetAllUsersQuery, GetUserResponse[]>
    {
        private readonly IUserRepository _userRepository = userRepository;
        public async Task<Result<GetUserResponse[]>> Execute(GetAllUsersQuery data)
        {
            var user = await _userRepository.GetAll(data);
            if (user == null)
            {
                return Result<GetUserResponse[]>.Failure(new UserNotFoundException());
            }

            var response = user.Select(user => new GetUserResponse(
                user.GetId(),
                user.GetName(),
                user.GetEmail(),
                user.GetPhone(),
                user.GetUserType(),
                user.GetIsActive(),
                user.GetDepartament()
                )
            ).ToArray();
            
            return Result<GetUserResponse[]>.Success(response);
        }
    }
}
