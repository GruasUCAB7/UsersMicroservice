using FluentValidation;
using FluentValidation.Results;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using UsersMS.src.Users.Application.Repositories;
using UsersMS.Core.Application.IdGenerator;
using UsersMS.src.Users.Application.Commands.CreateUser.Types;
using UsersMS.src.Users.Application.Commands.UpdateUser.Types;
using UsersMS.Core.Application.Logger;
using UsersMS.src.Users.Infrastructure.Controllers;
using UsersMS.Core.Application.EmailSender;
using UsersMS.Core.Application.EmailSender.Types;
using UsersMS.src.Users.Application.Queries.GetAllUsers.Types;
using UsersMS.src.Users.Domain;
using UsersMS.src.Users.Domain.ValueObjects;
using UsersMS.src.Users.Application.Queries.Types;
using UsersMS.src.Users.Application.Queries.GetById.Types;
using UsersMS.Core.Utils.Optional;
using UsersMS.Core.Utils.Result;

namespace UsersMS.Tests.Users.Infrastructure
{
    public class UserControllerTests
    {
        private readonly Mock<IUserRepository> _userRepoMock = new Mock<IUserRepository>();
        private readonly Mock<IdGenerator<string>> _idGeneratorMock = new Mock<IdGenerator<string>>();
        private readonly Mock<IValidator<CreateUserCommand>> _validatorCreateMock = new Mock<IValidator<CreateUserCommand>>();
        private readonly Mock<IValidator<UpdateUserCommand>> _validatorUpdateMock = new Mock<IValidator<UpdateUserCommand>>();
        private readonly Mock<ILoggerContract> _loggerMock = new Mock<ILoggerContract>();
        private readonly UserController _controller;
        private readonly Mock<IEmailSender> _emailServiceMock = new Mock<IEmailSender>();

        public UserControllerTests()
        {
            _controller = new UserController(_userRepoMock.Object, _idGeneratorMock.Object, _validatorCreateMock.Object, _validatorUpdateMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnCreated()
        {
            var command = new CreateUserClientCommand(
                "Juan",
                "juan@gmail.com",
                "+58 424-0000000",
                "Admin",
                "RRHH"
            );

            string temporaryPassword = Guid.NewGuid().ToString("n").Substring(0, 8);
            string subject = "Bienvenido a Gruas UCAB";
            string body = $@"
            <h1>¡Hola {command.Name}!</h1>
            <p>Gracias por registrarse en Gruas UCAB.</p>
            <p>Su contraseña temporal es: <b>{temporaryPassword}</b></p>
            <p>Por favor, inicie sesión con esta contraseña y cámbiela antes de 24 horas.</p>
            <p>Saludos,<br>El equipo de Gruas UCAB</p>";

            var emailSenderData = new EmailSenderDto(command.Email, subject, body);

            var userId = new CreateUserResponse("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");

            _idGeneratorMock.Setup(x => x.Generate()).Returns(userId.Id);

            _validatorCreateMock.Setup(x => x.Validate(It.IsAny<CreateUserCommand>())).Returns(new ValidationResult());

            _emailServiceMock.Setup(x => x.SendEmail(emailSenderData)).Returns(Task.CompletedTask);

            var result = await _controller.CreateUser(command, _emailServiceMock.Object);

            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, actionResult.StatusCode);
        }

        [Fact]
        public async Task CreateCrane_ShouldReturn400_WhenValidationFails()
        {
            var command = new CreateUserClientCommand(
               "Juan",
               "juan@gmail.com",
               "+58 424-0000000",
               "Admin",
               "RRHH"
           );
            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name is required") });
            _validatorCreateMock.Setup(x => x.Validate(It.IsAny<CreateUserCommand>())).Returns(validationResult);

            var result = await _controller.CreateUser(command, _emailServiceMock.Object) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result?.StatusCode);
            Assert.Equal(new List<string> { "Name is required" }, result?.Value);
        }

        [Fact]
        public async Task CreateUser_ShouldReturn409_WhenUserAlreadyExists()
        {
            var command = new CreateUserClientCommand(
               "Juan",
               "juan@gmail.com",
               "+58 424-0000000",
               "Admin",
               "RRHH"
           );

            var userId = new CreateUserResponse("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");
            _validatorCreateMock.Setup(x => x.Validate(It.IsAny<CreateUserCommand>())).Returns(new ValidationResult());
            _userRepoMock.Setup(x => x.ExistByEmail(command.Email)).ReturnsAsync(true);

            var result = await _controller.CreateUser(command, _emailServiceMock.Object);

            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(409, actionResult.StatusCode);
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturn200_WhenUsersAreRetrievedSuccessfully()
        {
            var query = new GetAllUsersQuery(10, 1, "active");
            var users = new List<User>
            {
                User.CreateUser(new UserId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e"), new UserName("Juan Perez"), new UserEmail("juan@gmail.com"), new UserPhone("+58 424-0000000"), new UserType("Admin"), new DeptoName("RRHH")),
                User.CreateUser(new UserId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f5w"), new UserName("Pedro Garcia"), new UserEmail("pedro@gmail.com"), new UserPhone("+58 424-0000001"), new UserType("Driver"), new DeptoName("Servicio")),
            };

            var userResponses = users.Select(u => new GetUserResponse(u.GetId(), u.GetName(), u.GetEmail(), u.GetPhone(), u.GetUserType(), u.GetIsActive(), u.GetDepartment(), u.GetTemporaryPassword(), u.GetPasswordExpirationDate())).ToArray();

            _userRepoMock.Setup(x => x.GetAll(query)).ReturnsAsync(users);

            var result = await _controller.GetAllUsers(query);

            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);

            var responseValue = Assert.IsType<GetUserResponse[]>(actionResult.Value);
            Assert.Equal(userResponses, responseValue);
        }

        [Fact]
        public async Task GetUserById_ShouldReturn200_WhenUserExist()
        {
            var userId = new UserId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");
            var name = new UserName("Juan Perez");
            var email = new UserEmail("juan@gmail.com");
            var phone = new UserPhone("+58 424-0000000");
            var type = new UserType("Admin");
            var depto = new DeptoName("RRHH");
            var existingUser = User.CreateUser(userId, name, email, phone, type, depto);

            var query = new GetUserByIdQuery(userId.GetValue());

            _userRepoMock.Setup(r => r.GetById(userId.GetValue())).ReturnsAsync(Optional<User>.Of(existingUser));

            var result = await _controller.GetUserById(userId.GetValue());

            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async Task GetUserById_ShouldReturn500_WhenUserNotFound()
        {
            var query = new GetUserByIdQuery("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");

            _userRepoMock.Setup(r => r.GetById("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e")).ReturnsAsync(Optional<User>.Empty());

            var result = await _controller.GetUserById(query.Id);

            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, actionResult.StatusCode);
        }

        [Fact]
        public async Task UpdateUpdate_ShouldReturn200_WhenUpdateIsSuccessful()
        {
            var userId = new UserId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");
            var name = new UserName("Juan Perez");
            var email = new UserEmail("juan@gmail.com");
            var phone = new UserPhone("+58 424-0000000");
            var type = new UserType("Admin");
            var depto = new DeptoName("RRHH");
            var existingUser = User.CreateUser(userId, name, email, phone, type, depto);

            var command = new UpdateUserCommand(true, "+58 424-0000002", "RRHH", "Operator", false, DateTime.UtcNow);

            _validatorUpdateMock.Setup(x => x.Validate(command)).Returns(new ValidationResult());

            _userRepoMock.Setup(r => r.GetById(userId.GetValue())).ReturnsAsync(Optional<User>.Of(existingUser));
            _userRepoMock.Setup(r => r.Update(existingUser)).ReturnsAsync(Result<User>.Success(existingUser));

            var result = await _controller.UpdateUser(command, userId.GetValue());

            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturn400_WhenValidationFails()
        {
            var command = new UpdateUserCommand(true, "+58 424-0000002", "RRHH", "Operator", false, DateTime.UtcNow);
            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("IsActive", "IsActive is required") });
            _validatorUpdateMock.Setup(x => x.Validate(command)).Returns(validationResult);

            var result = await _controller.UpdateUser(command, "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e") as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result?.StatusCode);
            Assert.Equal(new List<string> { "IsActive is required" }, result?.Value);
        }

        [Fact]
        public async Task UpdateCrane_ShouldReturn409_WhenCraneNotFound()
        {
            var command = new UpdateUserCommand(true, "+58 424-0000002", "RRHH", "Operator", false, DateTime.UtcNow);
            _validatorUpdateMock.Setup(x => x.Validate(command)).Returns(new ValidationResult());
            _userRepoMock.Setup(r => r.GetById("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e")).ReturnsAsync(Optional<User>.Empty());

            var result = await _controller.UpdateUser(command, "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");

            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(409, actionResult.StatusCode);
        }
    }
}
