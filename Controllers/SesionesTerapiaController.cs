using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using physiocare_backend.Data;
using physiocare_backend.Models;

namespace physiocare_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SesionesTerapiaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SesionesTerapiaController(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task<(string role, int? pacienteId, int? terapeutaId)> GetCurrentUserContext()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value 
                        ?? User.Claims.FirstOrDefault(c => c.Type.EndsWith("role", StringComparison.OrdinalIgnoreCase))?.Value 
                        ?? "";

            int? pacienteId = null;
            var pacClaim = User.Claims.FirstOrDefault(c => c.Type.Equals("PacienteId", StringComparison.OrdinalIgnoreCase))?.Value;
            if (int.TryParse(pacClaim, out int pId) && pId > 0)
            {
                pacienteId = pId;
            }

            int? terapeutaId = null;
            var terClaim = User.Claims.FirstOrDefault(c => c.Type.Equals("TerapeutaId", StringComparison.OrdinalIgnoreCase))?.Value;
            if (int.TryParse(terClaim, out int tId) && tId > 0)
            {
                terapeutaId = tId;
            }

            // Fallback resiliente a base de datos si falta el ID en el claim del token
            if ((userRole == "Paciente" && !pacienteId.HasValue) || (userRole == "Terapeuta" && !terapeutaId.HasValue))
            {
                var username = User.Identity?.Name 
                            ?? User.Claims.FirstOrDefault(c => c.Type.EndsWith("name", StringComparison.OrdinalIgnoreCase))?.Value;

                if (!string.IsNullOrEmpty(username))
                {
                    var userDb = await _context.Usuarios.FirstOrDefaultAsync(u => u.username == username);
                    if (userDb != null)
                    {
                        if (!pacienteId.HasValue && userDb.PacienteId.HasValue) pacienteId = userDb.PacienteId;
                        if (!terapeutaId.HasValue && userDb.TerapeutaId.HasValue) terapeutaId = userDb.TerapeutaId;
                    }
                }
            }

            return (userRole, pacienteId, terapeutaId);
        }

        // GET: api/SesionesTerapia
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SesionTerapia>>> GetSesionesTerapia()
        {
            var (userRole, pacienteId, terapeutaId) = await GetCurrentUserContext();

            var query = _context.SesionesTerapia
                .Include(s => s.Cita)
                    .ThenInclude(c => c.ServicioTerapia)
                .Include(s => s.Cita)
                    .ThenInclude(c => c.EstadoCita)
                .Include(s => s.Paciente)
                .Include(s => s.Terapeuta)
                    .ThenInclude(t => t.EspecialidadTerapeuta)
                .OrderByDescending(s => s.fechaSesion)
                .AsQueryable();

            if (userRole == "Paciente")
            {
                if (pacienteId.HasValue && pacienteId.Value > 0)
                {
                    query = query.Where(s => s.PacienteId == pacienteId.Value);
                }
                else
                {
                    return Ok(new List<SesionTerapia>());
                }

                var listPaciente = await query.ToListAsync();
                // Ocultar observaciones internas del terapeuta para el rol Paciente
                foreach (var sesion in listPaciente)
                {
                    sesion.observaciones = null;
                }
                return Ok(listPaciente);
            }
            else if (userRole == "Terapeuta")
            {
                if (terapeutaId.HasValue && terapeutaId.Value > 0)
                {
                    query = query.Where(s => s.TerapeutaId == terapeutaId.Value);
                }
                else
                {
                    return Ok(new List<SesionTerapia>());
                }
            }

            return await query.ToListAsync();
        }

        // GET: api/SesionesTerapia/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SesionTerapia>> GetSesionTerapia(int id)
        {
            var (userRole, pacienteId, terapeutaId) = await GetCurrentUserContext();

            var sesion = await _context.SesionesTerapia
                .Include(s => s.Cita)
                    .ThenInclude(c => c.ServicioTerapia)
                .Include(s => s.Cita)
                    .ThenInclude(c => c.EstadoCita)
                .Include(s => s.Paciente)
                .Include(s => s.Terapeuta)
                    .ThenInclude(t => t.EspecialidadTerapeuta)
                .FirstOrDefaultAsync(s => s.id == id);

            if (sesion == null)
            {
                return NotFound(new { message = "La sesión no existe." });
            }

            if (userRole == "Paciente")
            {
                if (!pacienteId.HasValue || sesion.PacienteId != pacienteId.Value)
                {
                    return Forbid();
                }
                // Ocultar observaciones internas al paciente
                sesion.observaciones = null;
            }
            else if (userRole == "Terapeuta")
            {
                if (!terapeutaId.HasValue || sesion.TerapeutaId != terapeutaId.Value)
                {
                    return Forbid();
                }
            }

            return sesion;
        }

        // POST: api/SesionesTerapia
        [HttpPost]
        [Authorize(Roles = "Administrador,Terapeuta")] // Solo terapeutas y administradores registran avances de sesión
        public async Task<ActionResult<SesionTerapia>> PostSesionTerapia(SesionTerapia sesion)
        {
            if (sesion == null)
            {
                return BadRequest(new { message = "Datos de sesión inválidos." });
            }

            // 1. Validar que la cita exista
            if (sesion.CitaId <= 0)
            {
                return BadRequest(new { message = "El ID de la cita vinculada es obligatorio." });
            }

            var cita = await _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Terapeuta)
                .Include(c => c.ServicioTerapia)
                .Include(c => c.EstadoCita)
                .FirstOrDefaultAsync(c => c.id == sesion.CitaId);

            if (cita == null)
            {
                return NotFound(new { message = "La cita especificada no existe." });
            }

            // 2. Validar que la cita pertenezca al terapeuta autenticado (salvo si es Administrador)
            var (userRole, _, terapeutaId) = await GetCurrentUserContext();

            if (userRole == "Terapeuta")
            {
                if (!terapeutaId.HasValue || cita.TerapeutaId != terapeutaId.Value)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "No tiene permiso para registrar una sesión en una cita asignada a otro terapeuta." });
                }
                sesion.TerapeutaId = terapeutaId.Value;
            }
            else
            {
                if (sesion.TerapeutaId <= 0)
                {
                    sesion.TerapeutaId = cita.TerapeutaId;
                }
            }

            // Asegurar que el paciente sea el de la cita
            sesion.PacienteId = cita.PacienteId;

            // 3. Validar estado de la sesión
            var estadosValidos = new[] { "Programada", "Completada", "Cancelada" };
            if (string.IsNullOrWhiteSpace(sesion.estado) || !estadosValidos.Contains(sesion.estado))
            {
                return BadRequest(new { message = "El estado debe ser 'Programada', 'Completada' o 'Cancelada'." });
            }

            if (sesion.fechaSesion == default)
            {
                sesion.fechaSesion = DateTime.UtcNow;
            }

            // Si la sesión se registra como Completada o Cancelada, sincronizar el estado de la cita
            if (sesion.estado == "Completada")
            {
                var estadoCompletada = await _context.EstadosCitas.FirstOrDefaultAsync(e => e.nombre == "Completada");
                if (estadoCompletada != null && cita.EstadoCitaId != estadoCompletada.id)
                {
                    cita.EstadoCitaId = estadoCompletada.id;
                }
            }
            else if (sesion.estado == "Cancelada")
            {
                var estadoCancelada = await _context.EstadosCitas.FirstOrDefaultAsync(e => e.nombre == "Cancelada");
                if (estadoCancelada != null && cita.EstadoCitaId != estadoCancelada.id)
                {
                    cita.EstadoCitaId = estadoCancelada.id;
                }
            }

            _context.SesionesTerapia.Add(sesion);
            await _context.SaveChangesAsync();

            var sesionCreada = await _context.SesionesTerapia
                .Include(s => s.Cita)
                    .ThenInclude(c => c.ServicioTerapia)
                .Include(s => s.Cita)
                    .ThenInclude(c => c.EstadoCita)
                .Include(s => s.Paciente)
                .Include(s => s.Terapeuta)
                .FirstOrDefaultAsync(s => s.id == sesion.id);

            return CreatedAtAction("GetSesionTerapia", new { id = sesion.id }, sesionCreada ?? sesion);
        }

        // PUT: api/SesionesTerapia/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Terapeuta")]
        public async Task<IActionResult> PutSesionTerapia(int id, SesionTerapia sesion)
        {
            if (id != sesion.id)
            {
                return BadRequest(new { message = "El ID de la ruta no coincide con el de la sesión." });
            }

            var sesionExistente = await _context.SesionesTerapia
                .Include(s => s.Cita)
                .FirstOrDefaultAsync(s => s.id == id);

            if (sesionExistente == null)
            {
                return NotFound(new { message = "La sesión especificada no existe." });
            }

            var (userRole, _, terapeutaId) = await GetCurrentUserContext();
            if (userRole == "Terapeuta")
            {
                if (!terapeutaId.HasValue || sesionExistente.TerapeutaId != terapeutaId.Value)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "No tiene permiso para editar sesiones de otro terapeuta." });
                }
            }

            var estadosValidos = new[] { "Programada", "Completada", "Cancelada" };
            if (!string.IsNullOrWhiteSpace(sesion.estado) && !estadosValidos.Contains(sesion.estado))
            {
                return BadRequest(new { message = "El estado debe ser 'Programada', 'Completada' o 'Cancelada'." });
            }

            if (sesion.fechaSesion != default) sesionExistente.fechaSesion = sesion.fechaSesion;
            sesionExistente.tratamientoRealizado = sesion.tratamientoRealizado;
            sesionExistente.observaciones = sesion.observaciones;
            sesionExistente.evolucion = sesion.evolucion;
            sesionExistente.recomendaciones = sesion.recomendaciones;
            if (!string.IsNullOrWhiteSpace(sesion.estado)) sesionExistente.estado = sesion.estado;

            if (sesionExistente.estado == "Completada" && sesionExistente.Cita != null)
            {
                var estadoCompletada = await _context.EstadosCitas.FirstOrDefaultAsync(e => e.nombre == "Completada");
                if (estadoCompletada != null && sesionExistente.Cita.EstadoCitaId != estadoCompletada.id)
                {
                    sesionExistente.Cita.EstadoCitaId = estadoCompletada.id;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SesionTerapiaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/SesionesTerapia/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador,Terapeuta")]
        public async Task<IActionResult> DeleteSesionTerapia(int id)
        {
            var sesion = await _context.SesionesTerapia.FindAsync(id);
            if (sesion == null)
            {
                return NotFound(new { message = "La sesión no existe." });
            }

            var (userRole, _, terapeutaId) = await GetCurrentUserContext();
            if (userRole == "Terapeuta")
            {
                if (!terapeutaId.HasValue || sesion.TerapeutaId != terapeutaId.Value)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "No tiene permiso para eliminar sesiones de otro terapeuta." });
                }
            }

            _context.SesionesTerapia.Remove(sesion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SesionTerapiaExists(int id)
        {
            return _context.SesionesTerapia.Any(e => e.id == id);
        }
    }
}
