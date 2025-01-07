using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UsersMS.src.Users.Application.Repositories;
using System.IdentityModel.Tokens.Jwt;
using UsersMS.src.Users.Infrastructure.Types;
using UsersMS.src.Users.Infrastructure.Services;

namespace UsersMS.src.Users.Infrastructure.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IUserRepository userRepository, JwtService jwtService) : ControllerBase
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly JwtService _jwtService = jwtService;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest loginRequest)
        {
            var userOptional = await _userRepository.GetByEmail(loginRequest.Email);

            if (!userOptional.HasValue)
            {
                return Unauthorized("Email o contraseña inválidos.");
            }

            var user = userOptional.Unwrap();

            if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.GetPasswordHash()))
            {
                return Unauthorized("Email o contraseña inválidos.");
            }

            if (user.GetTemporaryPassword())
            {
                if (user.GetPasswordExpirationDate() <= DateTime.UtcNow)
                {
                    return Unauthorized("La contraseña temporal ha expirado. Solicite una nueva contraseña.");
                }

                var limitedToken = _jwtService.GenerateLimitedToken(user.GetId(), user.GetEmail());

                return Ok(new
                {
                    RequiresPasswordChange = true,
                    LimitedToken = limitedToken,
                    Message = "Debe cambiar la contraseña temporal antes de continuar."
                });
            }

            var token = _jwtService.GenerateToken(user.GetId(), user.GetName(), user.GetEmail(), user.GetPhone(), user.GetTemporaryPassword(), user.GetUserType());

            return Ok(new
            {
                RequiresPasswordChange = false,
                Token = token
            });
        }


        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordRequest request)
        {
            var userOptional = await _userRepository.GetByEmail(request.Email);

            if (!userOptional.HasValue)
            {
                return Unauthorized("Usuario no encontrado.");
            }

            var user = userOptional.Unwrap();

            Console.WriteLine($"IsTemporaryPassword: {user.GetTemporaryPassword()}");
            Console.WriteLine($"PasswordExpirationDate: {user.GetPasswordExpirationDate()}");
            Console.WriteLine($"CurrentDateTime: {DateTime.UtcNow}");

            if (user.GetTemporaryPassword())
            {
                var expirationDate = user.GetPasswordExpirationDate();

                if (!expirationDate.HasValue)
                {
                    return BadRequest("No se encuentra la fecha de expiración de la contraseña temporal.");
                }

                if (expirationDate.Value <= DateTime.UtcNow)
                {
                    return Unauthorized("La contraseña temporal ha expirado. Debe solicitar una nueva contraseña temporal.");
                }
            }

            if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.GetPasswordHash()))
            {
                return Unauthorized("La contraseña actual es incorrecta.");
            }

            var hashedNewPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            user.SetPasswordHash(hashedNewPassword);
            user.SetTemporaryPassword(false);
            user.SetPasswordExpirationDate(DateTime.UtcNow.AddYears(100));

            var updateResult = await _userRepository.Update(user);

            if (updateResult.IsFailure)
            {
                return StatusCode(500, "Error al actualizar la contraseña.");
            }

            return Ok("La contraseña ha sido actualizada exitosamente.");
        }

        [HttpPost("reset-temporary-password")]
        [Authorize(Roles = "Admin,Operator")]
        public async Task<IActionResult> ResetTemporaryPassword([FromBody] UserResetPasswordRequest request)
        {
            try
            {
                var userOptional = await _userRepository.GetByEmail(request.Email);
                if (!userOptional.HasValue)
                {
                    return NotFound("Usuario no encontrado.");
                }

                var user = userOptional.Unwrap();

                if (!user.GetTemporaryPassword())
                {
                    return BadRequest("La contraseña actual no es temporal.");
                }

                string newTemporaryPassword = Guid.NewGuid().ToString("n").Substring(0, 8);
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newTemporaryPassword);

                user.SetPasswordHash(hashedPassword);
                user.SetPasswordExpirationDate(DateTime.UtcNow.AddHours(24));

                var updateResult = await _userRepository.Update(user);
                if (updateResult.IsFailure)
                {
                    return StatusCode(500, "No se pudo actualizar la contraseña temporal.");
                }

                return Ok(new { Message = "Se ha generado una nueva contraseña temporal.", TemporaryPassword = newTemporaryPassword });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        [FromServices] EmailService emailService)
        {
            try
            {
                var userOptional = await _userRepository.GetByEmail(request.Email);
                if (!userOptional.HasValue)
                {
                    return NotFound("Usuario no encontrado.");
                }

                var user = userOptional.Unwrap();

                if (user.GetName() != request.Name || user.GetPhone() != request.Phone)
                {
                    return Unauthorized("Los datos proporcionados no coinciden.");
                }

                string newTemporaryPassword = Guid.NewGuid().ToString("n").Substring(0, 8);
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newTemporaryPassword);

                user.SetPasswordHash(hashedPassword);
                user.SetTemporaryPassword(true);
                user.SetPasswordExpirationDate(DateTime.UtcNow.AddHours(1));

                var updateResult = await _userRepository.Update(user);
                if (updateResult.IsFailure)
                {
                    return StatusCode(500, "No se pudo generar la nueva contraseña temporal.");
                }

                string subject = "Restablecimiento de contraseña";
                string body = $@"
            <h1>Hola {user.GetName()},</h1>
            <p>Hemos generado una nueva contraseña temporal para ti.</p>
            <p><b>Contraseña temporal: {newTemporaryPassword}</b></p>
            <p>Esta contraseña es válida solo por 1 hora. Por favor, úsala para iniciar sesión y cámbiala inmediatamente.</p>
            <p>Saludos,<br>El equipo de Gruas UCAB</p>";

                await emailService.SendEmail(user.GetEmail(), subject, body);

                return Ok("Se ha enviado una nueva contraseña temporal a tu correo.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("update-password")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
        {
            var email = User.FindFirst("email")?.Value ??
                        User.FindFirst(JwtRegisteredClaimNames.Email)?.Value ??
                        User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized("El token JWT no contiene el correo del usuario.");
            }

            var userOptional = await _userRepository.GetByEmail(email);
            if (!userOptional.HasValue)
            {
                return NotFound("Usuario no encontrado.");
            }

            var user = userOptional.Unwrap();

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.GetPasswordHash()))
            {
                return BadRequest("La contraseña actual es incorrecta.");
            }

            var hashedNewPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            user.SetPasswordHash(hashedNewPassword);
            user.SetPasswordExpirationDate(DateTime.UtcNow.AddYears(100));

            var updateResult = await _userRepository.Update(user);

            if (updateResult.IsFailure)
            {
                return StatusCode(500, "Error al actualizar la contraseña.");
            }

            return Ok("La contraseña ha sido actualizada exitosamente.");
        }
    }
}

