using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using physiocare_backend.Data;
using physiocare_backend.Models;

namespace physiocare_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TerapeutasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TerapeutasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Terapeutas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Terapeuta>>> GetTerapeutas()
        {
            return await _context.Terapeutas
                .Include(t => t.EspecialidadTerapeuta)
                .Include(t => t.TipoDocumento)
                .ToListAsync();
        }

        // GET: api/Terapeutas/buscar/90807060
        // ESTE ES EL MÉTODO QUE USA EL FRONTEND PARA LAS CITAS
        [HttpGet("buscar/{numeroDocumento}")]
        public async Task<ActionResult<Terapeuta>> GetTerapeuta(long numeroDocumento)
        {
            var terapeuta = await _context.Terapeutas
                .Include(t => t.EspecialidadTerapeuta)
                .FirstOrDefaultAsync(t => t.numeroDocumento == numeroDocumento);

            if (terapeuta == null)
            {
                return NotFound();
            }

            return terapeuta;
        }

        // GET: api/Terapeutas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Terapeuta>> GetTerapeutaById(int id)
        {
            var terapeuta = await _context.Terapeutas
                .Include(t => t.EspecialidadTerapeuta)
                .FirstOrDefaultAsync(t => t.id == id);

            if (terapeuta == null)
            {
                return NotFound();
            }

            return terapeuta;
        }

        // PUT: api/Terapeutas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTerapeuta(int id, Terapeuta terapeuta)
        {
            if (id != terapeuta.id) return BadRequest();

            _context.Entry(terapeuta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TerapeutaExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // POST: api/Terapeutas
        [HttpPost]
        public async Task<ActionResult<Terapeuta>> PostTerapeuta(Terapeuta terapeuta)
        {
            _context.Terapeutas.Add(terapeuta);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTerapeutaById", new { id = terapeuta.id }, terapeuta);
        }

        // DELETE: api/Terapeutas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTerapeuta(int id)
        {
            var terapeuta = await _context.Terapeutas.FindAsync(id);
            if (terapeuta == null) return NotFound();

            _context.Terapeutas.Remove(terapeuta);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TerapeutaExists(int id)
        {
            return _context.Terapeutas.Any(e => e.id == id);
        }
    }
}