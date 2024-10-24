using Microsoft.AspNetCore.Mvc;
using UsersMS.src.Users.Application.Commands.CreateUser.Types;
using UsersMS.src.Users.Infrastructure.Dto;
using UsersMS.src.Users.Application.Commands.CreateUser;
using UsersMS.src.Users.Application.Repositories;
using UsersMS.Core.Application.IdGenerator;
using FluentValidation;
using UsersMS.Core.Application.Logger;

namespace UsersMS.src.Users.Infrastructure.Controllers
{

    [Route("api/users")]
    [ApiController]
    public class UserController(IUserRepository userRepo, IDeptoRepository deptoRepo, IdGenerator<string> idGenerator, IValidator<CreateUserCommand> validator, ILoggerContract logger)
    {
        private readonly IUserRepository _userRepo = userRepo;
        private readonly IDeptoRepository _deptoRepo = deptoRepo;
        private readonly IdGenerator<string> _idGenerator = idGenerator;
        private readonly IValidator<CreateUserCommand> _validator = validator;
        private readonly ILoggerContract _logger = logger;

        [HttpPost]
        public async Task<CreateUserResponse> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                var command = new CreateUserCommand(createUserDto.Name, createUserDto.Email, createUserDto.Phone, createUserDto.UserType, createUserDto.Departament);

                var result = _validator.Validate(command);
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.Error($"Validation failed for CreateUserCommand:", string.Join(", ", errors));
                    return new CreateUserResponse(string.Empty, errors);
                }

                var handler = new CreateUserCommandHandler(_userRepo, _deptoRepo, _idGenerator);
                var executionResult = await handler.Execute(command);

                if (executionResult.IsSuccessful)
                {
                    _logger.Log("User created successfully: {UserId}", executionResult.Unwrap().Id);
                    return executionResult.Unwrap();
                }
                else
                {
                    _logger.Error("Failed to create user: {ErrorMessage}", executionResult.ErrorMessage);
                    return new CreateUserResponse(string.Empty, [executionResult.ErrorMessage]);
                }
            }
            catch (Exception ex)
            { 
                _logger.Exception("An error occurred while creating the user.", ex.Message);
                return new CreateUserResponse(string.Empty, ["An internal server error occurred."]);
            }
        }  
    }
}
