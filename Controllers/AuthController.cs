using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using physiocare_backend.Data;
using physiocare_backend.Models;
using BCryptNet = BCrypt.Net.BCrypt; // Mapeo de la librería

namespace physiocare_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // 1. Buscar si el usuario existe
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.username == loginDto.username);

            if (usuario == null)
            {
                return Unauthorized(new { message = "Usuario o contraseña incorrectos" });
            }

            // 2. Verificar si la contraseña limpia coincide con el Hash de la BD
            bool passwordValido = BCryptNet.Verify(loginDto.password, usuario.passwordHash);

            if (!passwordValido)
            {
                return Unauthorized(new { message = "Usuario o contraseña incorrectos" });
            }

            // 3. Login exitoso (llave de paso directa)
            return Ok(new
            {
                success = true,
                message = "Bienvenido al sistema",
                username = usuario.username
            });
        }
    }
}
