using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using physiocare_backend.Data;
using physiocare_backend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCryptNet = BCrypt.Net.BCrypt;

namespace physiocare_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
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

            // NUEVO: Verificar si la cuenta está activa
            if (!usuario.activo)
            {
                return Unauthorized(new { message = "El usuario está inactivo. Contacte al administrador." });
            }

            // 2. Verificar si la contraseña limpia coincide con el Hash de la BD
            bool passwordValido = BCryptNet.Verify(loginDto.password, usuario.passwordHash);

            if (!passwordValido)
            {
                return Unauthorized(new { message = "Usuario o contraseña incorrectos" });
            }

            // 3. Generar JWT Token
            var token = GenerarJwtToken(usuario);

            // 4. Login exitoso
            return Ok(new
            {
                success = true,
                message = "Bienvenido al sistema",
                token = token,
                user = new 
                {
                    id = usuario.id,
                    username = usuario.username,
                    rol = usuario.rol,
                    pacienteId = usuario.PacienteId,
                    terapeutaId = usuario.TerapeutaId
                }
            });
        }

        // ENDPOINT TEMPORAL PARA PRUEBAS EN SWAGGER (Borrar en producción)
        [HttpGet("test-tokens")]
        public async Task<IActionResult> GetTestTokens()
        {
            // Buscamos a los usuarios de prueba que creamos en la base de datos
            var usuarios = await _context.Usuarios.ToListAsync();
            
            if (!usuarios.Any())
                return NotFound("No hay usuarios en la base de datos. ¿Ejecutaste las migraciones?");

            var tokensGenerados = new List<object>();

            foreach (var usuario in usuarios)
            {
                var token = GenerarJwtToken(usuario);
                tokensGenerados.Add(new 
                { 
                    Rol = usuario.rol, 
                    Usuario = usuario.username, 
                    TokenParaCopiar = $"Bearer {token}" // Ya viene con la palabra Bearer y el espacio
                });
            }

            return Ok(tokensGenerados);
        }

        private string GenerarJwtToken(Usuario usuario)
{
    var jwtSettings = _config.GetSection("Jwt");
    var keyString = jwtSettings["Key"] ?? "PhysioCare_Super_Secret_Key_2026_Minimum_32_Bytes_Long!";
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
    
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    // Definición explícita de claims
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, usuario.id.ToString()),
        new Claim(ClaimTypes.Name, usuario.username),
        new Claim(ClaimTypes.Role, usuario.rol ?? "Paciente")
    };

    // Mapeo seguro de llaves foráneas
    if (usuario.PacienteId.HasValue && usuario.PacienteId.Value > 0)
    {
        claims.Add(new Claim("PacienteId", usuario.PacienteId.Value.ToString()));
    }

    if (usuario.TerapeutaId.HasValue && usuario.TerapeutaId.Value > 0)
    {
        claims.Add(new Claim("TerapeutaId", usuario.TerapeutaId.Value.ToString()));
    }

    double expireDays = 1;
    if (double.TryParse(jwtSettings["ExpireDays"], out double configuredDays))
    {
        expireDays = configuredDays;
    }

    var token = new JwtSecurityToken(
        issuer: jwtSettings["Issuer"],
        audience: jwtSettings["Audience"],
        claims: claims,
        expires: DateTime.Now.AddDays(expireDays),
        signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}