using Moq;
using UsersMS.src.Users.Application.Queries.GetById;
using UsersMS.src.Users.Application.Queries.GetById.Types;
using UsersMS.src.Users.Application.Repositories;
using UsersMS.src.Users.Domain.ValueObjects;
using UsersMS.src.Users.Domain;
using Xunit;
using UsersMS.Core.Utils.Optional;

namespace UsersMS.Tests.Users.Application.Queries
{
    public class GetUserByIdQueryHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly GetUserByIdQueryHandler _handler;

        public GetUserByIdQueryHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new GetUserByIdQueryHandler(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task GetUserByIdSuccess()
        {
            var query = new GetUserByIdQuery("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");
            var user = User.CreateUser(
                new UserId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d"),
                new UserName("Juan"),
                new UserEmail("juan@gmail.com"),
                new UserPhone("+58 424-0000000"),
                new UserType("Admin"),
                new DeptoName("Administration")
            );

            _userRepositoryMock.Setup(x => x.GetById(query.Id)).ReturnsAsync(Optional<User>.Of(user));

            var result = await _handler.Execute(query);

            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task GetUserByIdWhenUserNotFound()
        {
            var query = new GetUserByIdQuery("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");

            _userRepositoryMock.Setup(x => x.GetById(query.Id)).ReturnsAsync(Optional<User>.Empty());

            var result = await _handler.Execute(query);

            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal("User not found", result.ErrorMessage);
        }
    }
}
