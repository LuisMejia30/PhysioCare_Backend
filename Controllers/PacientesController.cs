using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using physiocare_backend.Data;
using physiocare_backend.Models;

namespace physiocare_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Exige autenticación con Token JWT en todos los endpoints de este controlador
    public class PacientesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PacientesController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/Pacientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Paciente>>> GetPacientes()
        {
            return await _context.Pacientes.ToListAsync();
        }

        // GET: api/Pacientes/buscar/10203040
        // Método que usa el frontend en el módulo de Citas
        [HttpGet("buscar/{numeroDocumento}")]
        public async Task<ActionResult<Paciente>> GetPaciente(long numeroDocumento)
        {
            var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(p => p.numeroDocumento == numeroDocumento);

            if (paciente == null)
            {
                return NotFound();
            }

            return paciente;
        }

        // GET: api/Pacientes/5 (Búsqueda por ID interno)
        [HttpGet("{id}")]
        public async Task<ActionResult<Paciente>> GetPacienteById(int id)
        {
            var paciente = await _context.Pacientes.FindAsync(id);

            if (paciente == null)
            {
                return NotFound();
            }

            return paciente;
        }

        // GET: api/Pacientes/cargar-txt
        // Endpoint para leer información desde un archivo .txt en el servidor
        [HttpGet("cargar-txt")]
        public async Task<IActionResult> CargarArchivoTxt([FromQuery] string nombreArchivo = "informacion.txt")
        {
            // Busca el archivo dentro de la carpeta 'Archivos' en la raíz de la API
            string filePath = Path.Combine(_env.ContentRootPath, "Archivos", nombreArchivo);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { message = $"El archivo '{nombreArchivo}' no fue encontrado en el servidor." });
            }

            string contenido = await System.IO.File.ReadAllTextAsync(filePath);
            return Ok(new { exito = true, contenido = contenido });
        }

        // PUT: api/Pacientes/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Recepcionista,Terapeuta")]
        public async Task<IActionResult> PutPaciente(int id, Paciente paciente)
        {
            if (id != paciente.id) return BadRequest();

            _context.Entry(paciente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PacienteExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // POST: api/Pacientes
        [HttpPost]
        [Authorize(Roles = "Administrador,Recepcionista")]
        public async Task<ActionResult<Paciente>> PostPaciente(Paciente paciente)
        {
            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPacienteById", new { id = paciente.id }, paciente);
        }

        // DELETE: api/Pacientes/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador,Recepcionista")]
        public async Task<IActionResult> DeletePaciente(int id)
        {
            var paciente = await _context.Pacientes.FindAsync(id);
            if (paciente == null) return NotFound();

            _context.Pacientes.Remove(paciente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PacienteExists(int id)
        {
            return _context.Pacientes.Any(e => e.id == id);
        }
    }
}