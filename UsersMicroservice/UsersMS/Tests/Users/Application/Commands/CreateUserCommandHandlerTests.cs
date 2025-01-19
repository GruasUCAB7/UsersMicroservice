using Moq;
using UsersMS.Core.Application.IdGenerator;
using UsersMS.Core.Utils.Result;
using UsersMS.src.Users.Application.Commands.CreateUser;
using UsersMS.src.Users.Application.Commands.CreateUser.Types;
using UsersMS.src.Users.Application.Repositories;
using UsersMS.src.Users.Domain;
using UsersMS.src.Users.Domain.ValueObjects;
using Xunit;

namespace UsersMS.Tests.Users.Application.Commands
{
    public class CreateUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IdGenerator<string>> _idGeneratorMock;
        private readonly CreateUserCommandHandler _handler;

        public CreateUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _idGeneratorMock = new Mock<IdGenerator<string>>();
            _handler = new CreateUserCommandHandler(_userRepositoryMock.Object, _idGeneratorMock.Object);
        }

        [Fact]
        public async Task ShouldCreateUserSuccess()
        {
            var command = new CreateUserCommand(
                "Juan", 
                "juan@gmail.com", 
                "+58 424-0000000", 
                "Admin", 
                "RRHH", 
                "$2a$11$L6aBX25MZXm2i5EPGo.BWOauG7wKv7H2YPhAzqqzNtODj135bLfC.",
                false,
                DateTime.Now
            );

            _userRepositoryMock.Setup(x => x.ExistByEmail(command.Email)).ReturnsAsync(false);
            _idGeneratorMock.Setup(x => x.Generate()).Returns("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d");
            var result = await _handler.Execute(command);

            var user = User.CreateUser(
                new UserId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d"),
                new UserName(command.Name),
                new UserEmail(command.Email),
                new UserPhone(command.Phone),
                new UserType(command.UserType),
                new DeptoName(command.Department)
            );

            _userRepositoryMock.Setup(x => x.Save(user)).ReturnsAsync(Result<User>.Success(user));

            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task ShouldFailToCreateUserWhenUserAlreadyExists()
        {
            var command = new CreateUserCommand(
                "Juan",
                "juan@gmail.com",
                "+58 424-0000000",
                "Admin",
                "RRHH",
                "$2a$11$L6aBX25MZXm2i5EPGo.BWOauG7wKv7H2YPhAzqqzNtODj135bLfC.",
                false,
                DateTime.Now
            );

            _userRepositoryMock.Setup(x => x.ExistByEmail(command.Email)).ReturnsAsync(true);
            var result = await _handler.Execute(command);

            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal($"User with email juan@gmail.com already exists", result.ErrorMessage);
        }
    }
}