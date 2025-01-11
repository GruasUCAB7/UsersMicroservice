using Microsoft.AspNetCore.Mvc;
using UsersMS.src.Users.Application.Commands.CreateUser.Types;
using UsersMS.src.Users.Application.Commands.UpdateUser.Types;
using UsersMS.src.Users.Application.Commands.CreateUser;
using UsersMS.src.Users.Application.Repositories;
using UsersMS.Core.Application.IdGenerator;
using FluentValidation;
using UsersMS.Core.Application.Logger;
using UsersMS.src.Users.Application.Queries.GetAllUsers.Types;
using UsersMS.src.Users.Application.Queries.Types;
using UsersMS.src.Users.Application.Queries.GetAllUsers;
using UsersMS.src.Users.Application.Queries.GetById;
using UsersMS.src.Users.Application.Queries.GetById.Types;
using UsersMS.src.Users.Application.Commands.UpdateUser;
using Microsoft.AspNetCore.Authorization;
using UsersMS.src.Users.Infrastructure.Services;

namespace UsersMS.src.Users.Infrastructure.Controllers
{
    [Route("user")]
    [ApiController]
    public class UserController(
        
        IUserRepository userRepo,
        IdGenerator<string> idGenerator,
        IValidator<CreateUserCommand> validatorCreate,
        IValidator<UpdateUserCommand> validatorUpdate,
        ILoggerContract logger) : ControllerBase
    {
        private readonly IUserRepository _userRepo = userRepo;
        private readonly IdGenerator<string> _idGenerator = idGenerator;
        private readonly IValidator<CreateUserCommand> _validatorCreate = validatorCreate;
        private readonly IValidator<UpdateUserCommand> _validatorUpdate = validatorUpdate;
        private readonly ILoggerContract _logger = logger;


        [HttpPost]
        [Authorize(Roles = "Admin,Operator")]
        public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserClientCommand clientData,
        [FromServices] EmailService emailService)
        {
            try
            {
                string temporaryPassword = Guid.NewGuid().ToString("n").Substring(0, 8);
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(temporaryPassword);
                DateTime passwordExpirationDate = DateTime.UtcNow.AddHours(24);

                var command = new CreateUserCommand(
                    clientData.Name,
                    clientData.Email,
                    clientData.Phone,
                    clientData.UserType,
                    clientData.Department,
                    hashedPassword,
                    true,
                    passwordExpirationDate
                );

                var validate = _validatorCreate.Validate(command);
                if (!validate.IsValid)
                {
                    var errors = validate.Errors.Select(e => e.ErrorMessage).ToList();
                    return StatusCode(400, errors);
                }

                var handler = new CreateUserCommandHandler(_userRepo, _idGenerator);
                var result = await handler.Execute(command);

                if (result.IsSuccessful)
                {
                    string subject = "Bienvenido a Gruas UCAB";
                    string body = $@"
                    <h1>¡Hola {clientData.Name}!</h1>
                    <p>Gracias por registrarte en Gruas UCAB.</p>
                    <p>Tu contraseña temporal es: <b>{temporaryPassword}</b></p>
                    <p>Por favor, inicia sesión con esta contraseña y cámbiala antes de 24 horas.</p>
                    <p>Saludos,<br>El equipo de Gruas UCAB</p>";

                    await emailService.SendEmail(clientData.Email, subject, body);

                    return StatusCode(201, new { id = result.Unwrap().Id});
                }
                else
                {
                    return StatusCode(409, result.ErrorMessage);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.Exception("Unauthorized access attempt: {Message}", ex.Message);
                return StatusCode(403, "No tiene autorización para acceder a este recurso.");
            }
            catch (Exception ex)
            {
                _logger.Exception("An error occurred while creating the user.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }   



        [HttpGet]
        [Authorize(Roles = "Admin,Operator, Provider")]
        public async Task<IActionResult> GetAllUsers([FromQuery] GetAllUsersQuery data)
        {
            try
            {
                var query = new GetAllUsersQuery(data.PerPage, data.Page, data.IsActive);
                var handler = new GetAllUsersQueryHandler(_userRepo);
                var result = await handler.Execute(query);

                _logger.Log("List of users: {UserIds}", string.Join(", ", result.Unwrap().Select(u => u.Id)));
                return StatusCode(200, result.Unwrap());
            }
            catch (Exception ex)
            {
                _logger.Exception("Failed to get list of users", ex.Message);
                return StatusCode(200, Array.Empty<GetUserResponse>());
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Operator, Provider, Driver")]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var query = new GetUserByIdQuery(id);
                var handler = new GetUserByIdQueryHandler(_userRepo);
                var result = await handler.Execute(query);

                var user = result.Unwrap();

                Console.WriteLine(user);
                if (user == null || user.Id != id)
                {
                    _logger.Error("User not found: {UserId}", id);
                    return NotFound();
                }

                _logger.Log("User found: {UserId}", id);
                return StatusCode(200, user);
            }
            catch (Exception ex)
            {
                _logger.Exception("Failed to get user by id", ex.Message);
                return StatusCode(500, "User not found");
            }
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin, Operator, Provider, Driver")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserCommand data, string id)
        {
            try
            {
                var command = new UpdateUserCommand(data.IsActive, data.Phone, data.Department, data.UserType, data.IsTemporaryPassword, data.PasswordExpirationDate);

                var validate = _validatorUpdate.Validate(command);
                if (!validate.IsValid)
                {
                    var errors = validate.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.Error($"Validation failed for UpdateUserCommand: {string.Join(", ", errors)}");
                    return StatusCode(400, errors);
                }

                var handler = new UpdateUserCommandHandler(_userRepo);
                var result = await handler.Execute((id, data));
                if (result.IsSuccessful)
                {
                    var user = result.Unwrap();
                    _logger.Log("User updated: {UserId}", id);
                    return Ok(user);
                }
                else
                {
                    _logger.Error("Failed to update user: {ErrorMessage}", result.ErrorMessage);
                    return StatusCode(409, result.ErrorMessage);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.Error("Unauthorized access attempt: {Message}", ex.Message);
                return StatusCode(403, "No tiene autorización para acceder a este recurso.");
            }
            catch (Exception ex)
            {
                _logger.Exception("An error occurred while updating the user.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
