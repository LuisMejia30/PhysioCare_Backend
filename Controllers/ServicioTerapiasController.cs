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
            return await _context.Terapias.ToListAsync();
        }

        // GET: api/ServicioTerapias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServicioTerapia>> GetServicioTerapia(int id)
        {
            var servicioTerapia = await _context.Terapias.FindAsync(id);

            if (servicioTerapia == null)
            {
                return NotFound();
            }

            return servicioTerapia;
        }

        // PUT: api/ServicioTerapias/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
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
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ServicioTerapia>> PostServicioTerapia(ServicioTerapia servicioTerapia)
        {
            _context.Terapias.Add(servicioTerapia);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetServicioTerapia", new { id = servicioTerapia.id }, servicioTerapia);
        }

        // DELETE: api/ServicioTerapias/5
        [HttpDelete("{id}")]
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
