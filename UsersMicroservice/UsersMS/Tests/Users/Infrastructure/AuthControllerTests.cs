using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using UsersMS.Core.Application.EmailSender;
using UsersMS.Core.Application.EmailSender.Types;
using UsersMS.Core.Application.Services.JwtService;
using UsersMS.Core.Utils.Optional;
using UsersMS.Core.Utils.Result;
using UsersMS.src.Users.Application.Repositories;
using UsersMS.src.Users.Domain;
using UsersMS.src.Users.Domain.ValueObjects;
using UsersMS.src.Users.Infrastructure.Controllers;
using UsersMS.src.Users.Infrastructure.Types;
using Xunit;

namespace UsersMS.Tests.Users.Infrastructure
{
    public class AuthControllerTests
    {
        private readonly Mock<IUserRepository> _userRepoMock = new Mock<IUserRepository>();
        private readonly Mock<IJwtService> _jwtServiceMock = new Mock<IJwtService>();
        private readonly Mock<IEmailSender> _emailServiceMock = new Mock<IEmailSender>();
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _controller = new AuthController(_userRepoMock.Object, _jwtServiceMock.Object);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenCredentialsAreCorrect()
        {
            var loginRequest = new UserLoginRequest
            {
                Email = "juan@gmail.com",
                Password = "correctpassword"
            };

            var userId = "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e";
            var user = User.CreateUser(
                new UserId(userId),
                new UserName("Juan Perez"),
                new UserEmail("juan@gmail.com"),
                new UserPhone("+58 424-0000000"),
                new UserType("Admin"),
                new DeptoName("Administration")
            );

            user.SetPasswordHash(BCrypt.Net.BCrypt.HashPassword("correctpassword"));
            _userRepoMock.Setup(x => x.GetByEmail(loginRequest.Email)).ReturnsAsync(Optional<User>.Of(user));
            _jwtServiceMock.Setup(x => x.GenerateToken(userId, user.GetName(), user.GetEmail(), user.GetPhone(), user.GetTemporaryPassword(), user.GetUserType())).Returns("token");

            var result = await _controller.Login(loginRequest);

            var actionResult = Assert.IsType<OkObjectResult>(result);
            var response = actionResult.Value as dynamic;
            Assert.False(response.RequiresPasswordChange);
            Assert.Equal("token", response.Token);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenUserNotFound()
        {
            var loginRequest = new UserLoginRequest { Email = "juan@gmail.com", Password = "password" };
            _userRepoMock.Setup(x => x.GetByEmail(loginRequest.Email)).ReturnsAsync(Optional<User>.Empty());

            var result = await _controller.Login(loginRequest);

            var actionResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Email o contraseña inválidos.", actionResult.Value);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenPasswordIsIncorrect()
        {
            var loginRequest = new UserLoginRequest { Email = "juan@gmail.com", Password = "wrongpassword" };
            var userId = "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e";
            var user = User.CreateUser(
                new UserId(userId),
                new UserName("Juan Perez"),
                new UserEmail("juan@gmail.com"),
                new UserPhone("+58 424-0000000"),
                new UserType("Admin"),
                new DeptoName("Administration")
            );
            user.SetPasswordHash(BCrypt.Net.BCrypt.HashPassword("correctpassword"));
            _userRepoMock.Setup(x => x.GetByEmail(loginRequest.Email)).ReturnsAsync(Optional<User>.Of(user));

            var result = await _controller.Login(loginRequest);

            var actionResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Email o contraseña inválidos.", actionResult.Value);
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnOk_WhenPasswordIsChangedSuccessfully()
        {
            var request = new UserChangePasswordRequest { Email = "juan@gmail.com", OldPassword = "correctpassword", NewPassword = "newpassword" };
            var userId = "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e";
            var user = User.CreateUser(
                new UserId(userId),
                new UserName("Juan Perez"),
                new UserEmail("juan@gmail.com"),
                new UserPhone("+58 424-0000000"),
                new UserType("Admin"),
                new DeptoName("Administration")
            );

            user.SetPasswordHash(BCrypt.Net.BCrypt.HashPassword("correctpassword"));
            _userRepoMock.Setup(x => x.GetByEmail(request.Email)).ReturnsAsync(Optional<User>.Of(user));
            _userRepoMock.Setup(x => x.Update(user)).ReturnsAsync(Result<User>.Success(user));

            var result = await _controller.ChangePassword(request);

            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("La contraseña ha sido actualizada exitosamente.", actionResult.Value);
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnUnauthorized_WhenUserNotFound()
        {
            var request = new UserChangePasswordRequest { Email = "juan@gmail.com", OldPassword = "oldpassword", NewPassword = "newpassword" };
            _userRepoMock.Setup(x => x.GetByEmail(request.Email)).ReturnsAsync(Optional<User>.Empty());

            var result = await _controller.ChangePassword(request);

            var actionResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Usuario no encontrado.", actionResult.Value);
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnUnauthorized_WhenOldPasswordIsIncorrect()
        {
            var request = new UserChangePasswordRequest { Email = "juan@gmail.com", OldPassword = "wrongpassword", NewPassword = "newpassword" };
            var userId = "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e";
            var user = User.CreateUser(
               new UserId(userId),
               new UserName("Juan Perez"),
               new UserEmail("juan@gmail.com"),
               new UserPhone("+58 424-0000000"),
               new UserType("Admin"),
               new DeptoName("Administration")
           );
            user.SetPasswordHash(BCrypt.Net.BCrypt.HashPassword("correctpassword"));
            _userRepoMock.Setup(x => x.GetByEmail(request.Email)).ReturnsAsync(Optional<User>.Of(user));

            var result = await _controller.ChangePassword(request);

            var actionResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("La contraseña actual es incorrecta.", actionResult.Value);
        }

        [Fact]
        public async Task ResetTemporaryPassword_ShouldReturnOk_WhenPasswordIsResetSuccessfully()
        {
            var request = new UserResetPasswordRequest { Email = "juan@gmail.com" };
            var userId = "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e";
            var user = User.CreateUser(
               new UserId(userId),
               new UserName("Juan Perez"),
               new UserEmail("juan@gmail.com"),
               new UserPhone("+58 424-0000000"),
               new UserType("Admin"),
               new DeptoName("Administration")
           );

            user.SetPasswordHash(BCrypt.Net.BCrypt.HashPassword("correctpassword"));
            user.SetTemporaryPassword(true);
            _userRepoMock.Setup(x => x.GetByEmail(request.Email)).ReturnsAsync(Optional<User>.Of(user));
            _userRepoMock.Setup(x => x.Update(user)).ReturnsAsync(Result<User>.Success(user));

            var result = await _controller.ResetTemporaryPassword(request);

            var actionResult = Assert.IsType<OkObjectResult>(result);
            var response = actionResult.Value as dynamic;
            Assert.Equal("Se ha generado una nueva contraseña temporal.", response.Message);
        }

        [Fact]
        public async Task ResetTemporaryPassword_ShouldReturnNotFound_WhenUserNotFound()
        {
            var request = new UserResetPasswordRequest { Email = "juan@gmail.com" };
            _userRepoMock.Setup(x => x.GetByEmail(request.Email)).ReturnsAsync(Optional<User>.Empty());

            var result = await _controller.ResetTemporaryPassword(request);

            var actionResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Usuario no encontrado.", actionResult.Value);
        }

        [Fact]
        public async Task ResetTemporaryPassword_ShouldReturnBadRequest_WhenPasswordIsNotTemporary()
        {
            var request = new UserResetPasswordRequest { Email = "juan@gmail.com" };
            var userId = "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e";
            var user = User.CreateUser(
               new UserId(userId),
               new UserName("Juan Perez"),
               new UserEmail("juan@gmail.com"),
               new UserPhone("+58 424-0000000"),
               new UserType("Admin"),
               new DeptoName("Administration")
           );

            user.SetPasswordHash(BCrypt.Net.BCrypt.HashPassword("correctpassword"));
            user.SetTemporaryPassword(false);
            _userRepoMock.Setup(x => x.GetByEmail(request.Email)).ReturnsAsync(Optional<User>.Of(user));

            var result = await _controller.ResetTemporaryPassword(request);

            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("La contraseña actual no es temporal.", actionResult.Value);
        }

        [Fact]
        public async Task ForgotPassword_ShouldReturnOk_WhenPasswordIsResetSuccessfully()
        {
            var request = new ForgotPasswordRequest { Email = "juan@gmail.com", Name = "Juan Perez", Phone = "+58 424-0000000" };
            var userId = "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e";
            var user = User.CreateUser(
               new UserId(userId),
               new UserName("Juan Perez"),
               new UserEmail("juan@gmail.com"),
               new UserPhone("+58 424-0000000"),
               new UserType("Admin"),
               new DeptoName("Administration")
           );

            string newTemporaryPassword = Guid.NewGuid().ToString("n").Substring(0, 8);
            string subject = "Restablecimiento de contraseña";
            string body = $@"
                <h1>Hola {user.GetName()},</h1>
                <p>Hemos generado una nueva contraseña temporal para ti.</p>
                <p><b>Contraseña temporal: {newTemporaryPassword}</b></p>
                <p>Esta contraseña es válida solo por 1 hora. Por favor, úsaela para iniciar sesión y cámbiela inmediatamente.</p>
                <p>Saludos,<br>El equipo de Gruas UCAB</p>";
            var emailSenderData = new EmailSenderDto(user.GetEmail(), subject, body);

            _userRepoMock.Setup(x => x.GetByEmail(request.Email)).ReturnsAsync(Optional<User>.Of(user));
            _userRepoMock.Setup(x => x.Update(user)).ReturnsAsync(Result<User>.Success(user));
            _emailServiceMock.Setup(x => x.SendEmail(emailSenderData)).Returns(Task.CompletedTask);

            var result = await _controller.ForgotPassword(request, _emailServiceMock.Object);

            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Se ha enviado una nueva contraseña temporal a tu correo.", actionResult.Value);
        }

        [Fact]
        public async Task ForgotPassword_ShouldReturnNotFound_WhenUserNotFound()
        {
            var request = new ForgotPasswordRequest { Email = "juan@gmail.com", Name = "Juan Perez", Phone = "+58 424-0000000" };
            _userRepoMock.Setup(x => x.GetByEmail(request.Email)).ReturnsAsync(Optional<User>.Empty());

            var result = await _controller.ForgotPassword(request, _emailServiceMock.Object);

            var actionResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Usuario no encontrado.", actionResult.Value);
        }

        [Fact]
        public async Task ForgotPassword_ShouldReturnUnauthorized_WhenUserDataDoesNotMatch()
        {
            var request = new ForgotPasswordRequest { Email = "pedro@gmail.com", Name = "Pedro Garcia", Phone = "+58 424-0000001" };
            var userId = "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e";
            var user = User.CreateUser(
               new UserId(userId),
               new UserName("Juan Perez"),
               new UserEmail("juan@gmail.com"),
               new UserPhone("+58 424-0000000"),
               new UserType("Admin"),
               new DeptoName("Administration")
           );

            _userRepoMock.Setup(x => x.GetByEmail(request.Email)).ReturnsAsync(Optional<User>.Of(user));

            var result = await _controller.ForgotPassword(request, _emailServiceMock.Object);

            var actionResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Los datos proporcionados no coinciden.", actionResult.Value);
        }

        [Fact]
        public async Task UpdatePassword_ShouldReturnOk_WhenPasswordIsUpdatedSuccessfully()
        {
            var request = new UpdatePasswordRequest { CurrentPassword = "correctpassword", NewPassword = "newpassword" };
            var userId = "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e";
            var user = User.CreateUser(
               new UserId(userId),
               new UserName("Juan Perez"),
               new UserEmail("juan@gmail.com"),
               new UserPhone("+58 424-0000000"),
               new UserType("Admin"),
               new DeptoName("Administration")
           );

            user.SetPasswordHash(BCrypt.Net.BCrypt.HashPassword("correctpassword"));
            _userRepoMock.Setup(x => x.GetByEmail(user.GetEmail())).ReturnsAsync(Optional<User>.Of(user));
            _userRepoMock.Setup(x => x.Update(user)).ReturnsAsync(Result<User>.Success(user));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.GetEmail())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var result = await _controller.UpdatePassword(request);

            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("La contraseña ha sido actualizada exitosamente.", actionResult.Value);
        }

        [Fact]
        public async Task UpdatePassword_ShouldReturnUnauthorized_WhenEmailNotInToken()
        {
            var request = new UpdatePasswordRequest { CurrentPassword = "currentpassword", NewPassword = "newpassword" };
            var userId = "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e";
            var user = User.CreateUser(
               new UserId(userId),
               new UserName("Juan Perez"),
               new UserEmail("juan@gmail.com"),
               new UserPhone("+58 424-0000000"),
               new UserType("Admin"),
               new DeptoName("Administration")
           );
            _userRepoMock.Setup(x => x.GetByEmail(user.GetEmail())).ReturnsAsync(Optional<User>.Of(user));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var result = await _controller.UpdatePassword(request);

            var actionResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("El token JWT no contiene el correo del usuario.", actionResult.Value);
        }

        [Fact]
        public async Task UpdatePassword_ShouldReturnNotFound_WhenUserNotFound()
        {
            var request = new UpdatePasswordRequest { CurrentPassword = "currentpassword", NewPassword = "newpassword" };
            var userId = "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e";
            var user = User.CreateUser(
               new UserId(userId),
               new UserName("Juan Perez"),
               new UserEmail("juan@gmail.com"),
               new UserPhone("+58 424-0000000"),
               new UserType("Admin"),
               new DeptoName("Administration")
           );
            _userRepoMock.Setup(repo => repo.GetByEmail(It.IsAny<string>())).ReturnsAsync(Optional<User>.Empty());

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, "juan@gmail.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var result = await _controller.UpdatePassword(request);

            var actionResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Usuario no encontrado.", actionResult.Value);
        }

        [Fact]
        public async Task UpdatePassword_ShouldReturnBadRequest_WhenCurrentPasswordIsIncorrect()
        {
            var request = new UpdatePasswordRequest { CurrentPassword = "wrongpassword", NewPassword = "newpassword" };
            var userId = "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e";
            var user = User.CreateUser(
               new UserId(userId),
               new UserName("Juan Perez"),
               new UserEmail("juan@gmail.com"),
               new UserPhone("+58 424-0000000"),
               new UserType("Admin"),
               new DeptoName("Administration")
           );

            user.SetPasswordHash(BCrypt.Net.BCrypt.HashPassword("correctpassword"));
            _userRepoMock.Setup(repo => repo.GetByEmail(user.GetEmail())).ReturnsAsync(Optional<User>.Of(user));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, "juan@gmail.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var result = await _controller.UpdatePassword(request);

            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("La contraseña actual es incorrecta.", actionResult.Value);
        }
    }
}