using Xunit;
using Moq;
using UsersMS.src.Users.Application.Repositories;
using UsersMS.src.Users.Application.Queries.GetAllUsers.Types;
using UsersMS.src.Users.Application.Queries.GetAll;
using UsersMS.src.Users.Domain;
using UsersMS.src.Users.Domain.ValueObjects;


namespace UsersMS.Tests.Users.Application.Queries
{
    public class GetAllUsersQueryHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly GetAllUsersQueryHandler _handler;

        public GetAllUsersQueryHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new GetAllUsersQueryHandler(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllUsersSuccess()
        {
            var query = new GetAllUsersQuery(1, 5, "");
            var users = new List<User>
                {
                   User.CreateUser(
                        new UserId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d"),
                        new UserName("Juan"),
                        new UserEmail("juan@gmail.com"),
                        new UserPhone("+58 424-0000000"),
                        new UserType("Admin"),
                        new DeptoName("Administration")
                    ),
                    User.CreateUser(
                        new UserId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d"),
                        new UserName("Pedro"),
                        new UserEmail("pedro@gmail.com"),
                        new UserPhone("+58 424-0000001"),
                        new UserType("Operator"),
                        new DeptoName("RRHH")
                    )
                };


            _userRepositoryMock.Setup(x => x.GetAll(query)).ReturnsAsync(users);
            var result = await _handler.Execute(query);

            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
            Assert.Equal(2, result.Unwrap().Length);
        }

        [Fact]
        public async Task GetAllUsersFailureWhenNoUsersFound()
        {
            var query = new GetAllUsersQuery(1, 5, "");

            _userRepositoryMock.Setup(x => x.GetAll(query)).ReturnsAsync(new List<User>());

            var result = await _handler.Execute(query);

            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
            Assert.Empty(result.Unwrap());
        }
    }
}
