using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using physiocare_backend.Data;
using physiocare_backend.Models;
using BCryptNet = BCrypt.Net.BCrypt;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace physiocare_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (loginDto == null || string.IsNullOrWhiteSpace(loginDto.username) || string.IsNullOrWhiteSpace(loginDto.password))
                {
                    return BadRequest(new { message = "Usuario y contraseña requeridos" });
                }

                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.username == loginDto.username);

                if (usuario == null)
                {
                    return Unauthorized(new { message = "Usuario o contraseña incorrectos" });
                }

                if (!usuario.activo)
                {
                    return Unauthorized(new { message = "El usuario está inactivo. Contacte al administrador." });
                }

                bool passwordValido = BCryptNet.Verify(loginDto.password, usuario.passwordHash);

                if (!passwordValido)
                {
                    return Unauthorized(new { message = "Usuario o contraseña incorrectos" });
                }

                // 1. Generamos el token de forma segura
                var token = GenerateJwtToken(usuario);

                // 2. Retornamos la estructura que el frontend espera
                return Ok(new
                {
                    success = true,
                    message = "Bienvenido al sistema",
                    token = token,
                    user = new 
                    {
                        id = usuario.id,
                        username = usuario.username,
                        rol = usuario.Rol
                    }
                });
            }
            catch (Exception ex)
            {
                // Devolvemos el error detallado para diagnosticar la causa exacta en el frontend
                return StatusCode(500, new
                {
                    error = ex.Message,
                    innerError = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        private string GenerateJwtToken(Usuario usuario)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.username),
                new Claim(ClaimTypes.Role, string.IsNullOrEmpty(usuario.Rol) ? "Paciente" : usuario.Rol),
                new Claim("id", usuario.id.ToString())
            };

            var keyString = _configuration["Jwt:Key"]
                         ?? _configuration["Jwt__Key"]
                         ?? Environment.GetEnvironmentVariable("Jwt__Key")
                         ?? Environment.GetEnvironmentVariable("Jwt:Key");

            if (string.IsNullOrWhiteSpace(keyString) || keyString.Length < 32)
            {
                keyString = "PhysioCare_Super_Secret_Key_2026_Minimum_32_Bytes_Long!";
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var issuer = _configuration["Jwt:Issuer"]
                      ?? _configuration["Jwt__Issuer"]
                      ?? Environment.GetEnvironmentVariable("Jwt__Issuer")
                      ?? Environment.GetEnvironmentVariable("Jwt:Issuer")
                      ?? "PhysioCare";

            var audience = _configuration["Jwt:Audience"]
                        ?? _configuration["Jwt__Audience"]
                        ?? Environment.GetEnvironmentVariable("Jwt__Audience")
                        ?? Environment.GetEnvironmentVariable("Jwt:Audience")
                        ?? "PhysioCareUsers";

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}