using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ServicioTerapiasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ServicioTerapiasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ServicioTerapias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServicioTerapia>>> GetTerapias()
        {
            // Se añade .Include() para cargar la categoría en la respuesta
            // Esto soluciona el error en Reportes donde todas las citas aparecían como "General"
            return await _context.Terapias
                .Include(t => t.CategoriaTerapia) 
                .ToListAsync();
        }

        // GET: api/ServicioTerapias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServicioTerapia>> GetServicioTerapia(int id)
        {
            // Se reemplaza FindAsync por FirstOrDefaultAsync para permitir el uso de .Include()
            var servicioTerapia = await _context.Terapias
                .Include(t => t.CategoriaTerapia)
                .FirstOrDefaultAsync(t => t.id == id);

            if (servicioTerapia == null)
            {
                return NotFound();
            }

            return servicioTerapia;
        }

        // PUT: api/ServicioTerapias/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> PutServicioTerapia(int id, ServicioTerapia servicioTerapia)
        {
            if (id != servicioTerapia.id)
            {
                return BadRequest();
            }

            _context.Entry(servicioTerapia).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServicioTerapiaExists(id))
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

        // POST: api/ServicioTerapias
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ServicioTerapia>> PostServicioTerapia(ServicioTerapia servicioTerapia)
        {
            _context.Terapias.Add(servicioTerapia);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetServicioTerapia", new { id = servicioTerapia.id }, servicioTerapia);
        }

        // DELETE: api/ServicioTerapias/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteServicioTerapia(int id)
        {
            var servicioTerapia = await _context.Terapias.FindAsync(id);
            if (servicioTerapia == null)
            {
                return NotFound();
            }

            _context.Terapias.Remove(servicioTerapia);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServicioTerapiaExists(int id)
        {
            return _context.Terapias.Any(e => e.id == id);
        }
    }
}