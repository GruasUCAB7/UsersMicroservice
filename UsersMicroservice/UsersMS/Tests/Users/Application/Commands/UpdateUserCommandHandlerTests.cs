using Moq;
using UsersMS.src.Users.Application.Commands.UpdateUser;
using UsersMS.src.Users.Application.Commands.UpdateUser.Types;
using UsersMS.src.Users.Application.Repositories;
using UsersMS.src.Users.Domain.ValueObjects;
using UsersMS.src.Users.Domain;
using Xunit;
using UsersMS.Core.Utils.Optional;
using UsersMS.Core.Utils.Result;
using UsersMS.src.Users.Application.Exceptions;

namespace UsersMS.Tests.Users.Application.Commands
{
    public class UpdateUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UpdateUserCommandHandler _handler;

        public UpdateUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new UpdateUserCommandHandler(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task ShouldUpdateUserSucces()
        {
            var userId = "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d";
            var command = new UpdateUserCommand(true, "+58 424-0000001", "Tecnologia", "Operator", false, DateTime.UtcNow);

            var user = User.CreateUser(
                new UserId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d"),
                new UserName("Juan"),
                new UserEmail("juan@gmail.com"),
                new UserPhone(command.Phone),
                new UserType(command.UserType),
                new DeptoName(command.Department)
            );

            _userRepositoryMock.Setup(x => x.GetById(userId)).ReturnsAsync(Optional<User>.Of(user));
            _userRepositoryMock.Setup(x => x.Update(user)).ReturnsAsync(Result<User>.Success(user));

            var result = await _handler.Execute((userId, command));
            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task ShouldFailToUpdateUserWhenUserNotFound()
        {
            var userId = "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d";
            var command = new UpdateUserCommand(true, "+58 424-0000001", "Tecnologia", "Operator", false, DateTime.UtcNow);

            _userRepositoryMock.Setup(x => x.GetById(userId)).ReturnsAsync(Optional<User>.Empty());

            var result = await _handler.Execute((userId, command));

            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal("User not found", result.ErrorMessage);
        }

        [Fact]
        public async Task ShouldFailToUpdateUserWhenUpdateFails()
        {
            var userId = "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d";
            var command = new UpdateUserCommand(true, "+58 424-0000001", "Tecnologia", "Operator", false, DateTime.UtcNow);

            var user = User.CreateUser(
                new UserId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d"),
                new UserName("Juan"),
                new UserEmail("juan@gmail.com"),
                new UserPhone(command.Phone),
                new UserType(command.UserType),
                new DeptoName(command.Department)
            );

            _userRepositoryMock.Setup(x => x.GetById(userId)).ReturnsAsync(Optional<User>.Of(user));
            _userRepositoryMock.Setup(x => x.Update(user)).ReturnsAsync(Result<User>.Failure(new UserUpdateFailedException()));

            var result = await _handler.Execute((userId, command));

            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal("The user could not be updated correctly", result.ErrorMessage);
        }
    }
}
