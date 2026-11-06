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
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.username == loginDto.username);

            if (usuario == null)
            {
                return Unauthorized(new { message = "Usuario o contraseña incorrectos" });
            }

            bool passwordValido = BCryptNet.Verify(loginDto.password, usuario.passwordHash);

            if (!passwordValido)
            {
                return Unauthorized(new { message = "Usuario o contraseña incorrectos" });
            }

            var token = GenerateJwtToken(usuario.username);

            return Ok(new
            {
                success = true,
                message = "Bienvenido al sistema",
                username = usuario.username,
                token
            });
        }

        private string GenerateJwtToken(string username)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "MiClaveSuperSecretaPhysioCare123!"));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "PhysioCare",
                audience: _configuration["Jwt:Audience"] ?? "PhysioCareUsers",
                claims: claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}