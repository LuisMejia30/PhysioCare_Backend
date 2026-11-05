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

        // GET: api/SesionesTerapia
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SesionTerapia>>> GetSesionesTerapia()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var query = _context.SesionesTerapia
                .Include(s => s.Cita)
                .Include(s => s.Paciente)
                .Include(s => s.Terapeuta)
                .AsQueryable();

            if (userRole == "Paciente")
            {
                var pacienteIdClaim = User.FindFirst("PacienteId")?.Value;
                if (int.TryParse(pacienteIdClaim, out int pacienteId))
                {
                    query = query.Where(s => s.PacienteId == pacienteId);
                }
                else
                {
                    return Ok(new List<SesionTerapia>());
                }
            }
            else if (userRole == "Terapeuta")
            {
                var terapeutaIdClaim = User.FindFirst("TerapeutaId")?.Value;
                if (int.TryParse(terapeutaIdClaim, out int terapeutaId))
                {
                    query = query.Where(s => s.TerapeutaId == terapeutaId);
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
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var sesion = await _context.SesionesTerapia
                .Include(s => s.Cita)
                .Include(s => s.Paciente)
                .Include(s => s.Terapeuta)
                .FirstOrDefaultAsync(s => s.id == id);

            if (sesion == null)
            {
                return NotFound();
            }

            if (userRole == "Paciente")
            {
                var pacienteIdClaim = User.FindFirst("PacienteId")?.Value;
                if (!int.TryParse(pacienteIdClaim, out int pacienteId) || sesion.PacienteId != pacienteId)
                {
                    return Forbid();
                }
            }
            else if (userRole == "Terapeuta")
            {
                var terapeutaIdClaim = User.FindFirst("TerapeutaId")?.Value;
                if (!int.TryParse(terapeutaIdClaim, out int terapeutaId) || sesion.TerapeutaId != terapeutaId)
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
            _context.SesionesTerapia.Add(sesion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSesionTerapia", new { id = sesion.id }, sesion);
        }

        // PUT: api/SesionesTerapia/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Terapeuta")]
        public async Task<IActionResult> PutSesionTerapia(int id, SesionTerapia sesion)
        {
            if (id != sesion.id)
            {
                return BadRequest();
            }

            _context.Entry(sesion).State = EntityState.Modified;

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

        private bool SesionTerapiaExists(int id)
        {
            return _context.SesionesTerapia.Any(e => e.id == id);
        }
    }
}