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
    [Authorize] // Exige estar autenticado mediante Token JWT para cualquier endpoint
    public class CitasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CitasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Citas
        // Retorna las citas según el rol del usuario autenticado
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cita>>> GetCitas()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var query = _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Terapeuta)
                .Include(c => c.ServicioTerapia)
                .Include(c => c.EstadoCita)
                .AsQueryable();

            // Filtrado por Rol
            if (userRole == "Paciente")
            {
                var pacienteIdClaim = User.FindFirst("PacienteId")?.Value;
                if (int.TryParse(pacienteIdClaim, out int pacienteId))
                {
                    query = query.Where(c => c.PacienteId == pacienteId);
                }
                else
                {
                    return Ok(new List<Cita>()); // Si no hay PacienteId vinculado en el token
                }
            }
            else if (userRole == "Terapeuta")
            {
                var terapeutaIdClaim = User.FindFirst("TerapeutaId")?.Value;
                if (int.TryParse(terapeutaIdClaim, out int terapeutaId))
                {
                    query = query.Where(c => c.TerapeutaId == terapeutaId);
                }
                else
                {
                    return Ok(new List<Cita>()); // Si no hay TerapeutaId vinculado en el token
                }
            }
            // "Administrador" y "Recepcionista" continúan sin filtro para obtener todas las citas

            return await query.ToListAsync();
        }

        // GET: api/Citas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cita>> GetCita(int id)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var cita = await _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Terapeuta)
                .Include(c => c.ServicioTerapia)
                .Include(c => c.EstadoCita)
                .FirstOrDefaultAsync(c => c.id == id);

            if (cita == null)
            {
                return NotFound();
            }

            // Validar que un Paciente o Terapeuta solo acceda a sus propias citas
            if (userRole == "Paciente")
            {
                var pacienteIdClaim = User.FindFirst("PacienteId")?.Value;
                if (!int.TryParse(pacienteIdClaim, out int pacienteId) || cita.PacienteId != pacienteId)
                {
                    return Forbid();
                }
            }
            else if (userRole == "Terapeuta")
            {
                var terapeutaIdClaim = User.FindFirst("TerapeutaId")?.Value;
                if (!int.TryParse(terapeutaIdClaim, out int terapeutaId) || cita.TerapeutaId != terapeutaId)
                {
                    return Forbid();
                }
            }

            return cita;
        }

        // PUT: api/Citas/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Recepcionista,Terapeuta")] // Modificación restringida a roles de gestión
        public async Task<IActionResult> PutCita(int id, Cita cita)
        {
            if (id != cita.id)
            {
                return BadRequest();
            }

            _context.Entry(cita).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CitaExists(id))
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

        // POST: api/Citas
        [HttpPost]
        [Authorize(Roles = "Administrador,Recepcionista,Paciente")] // Pacientes, Recepcionistas y Admins pueden crear citas
        public async Task<ActionResult<Cita>> PostCita(Cita cita)
        {
            // Validar que el servicio realmente existe
            var servicio = await _context.Terapias.FindAsync(cita.ServicioTerapiaId);
            if (servicio == null) return BadRequest("El servicio seleccionado no existe.");

            // Si el rol es Paciente, forzar que la cita creada pertenezca a su propio PacienteId
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole == "Paciente")
            {
                var pacienteIdClaim = User.FindFirst("PacienteId")?.Value;
                if (int.TryParse(pacienteIdClaim, out int pacienteId))
                {
                    cita.PacienteId = pacienteId;
                }
                else
                {
                    return BadRequest("Su usuario no está vinculado a un perfil de paciente válido.");
                }
            }

            _context.Citas.Add(cita);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCita", new { id = cita.id }, cita);
        }

        // DELETE: api/Citas/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador,Recepcionista")] // Solo Administradores y Recepcionistas pueden cancelar/eliminar físicamente
        public async Task<IActionResult> DeleteCita(int id)
        {
            var cita = await _context.Citas.FindAsync(id);
            if (cita == null)
            {
                return NotFound();
            }

            _context.Citas.Remove(cita);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CitaExists(int id)
        {
            return _context.Citas.Any(e => e.id == id);
        }
    }
}