using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using physiocare_backend.Data;
using physiocare_backend.Models;

namespace physiocare_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CatalogosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Catalogos/Generos
        [HttpGet("Generos")]
        public async Task<ActionResult<IEnumerable<Genero>>> GetGeneros()
        {
            return await _context.Generos.OrderBy(g => g.nombre).ToListAsync();
        }

        // GET: api/Catalogos/TiposSangre
        [HttpGet("TiposSangre")]
        public async Task<ActionResult<IEnumerable<TipoSangre>>> GetTiposSangre()
        {
            return await _context.TiposSangre.ToListAsync();
        }

        // GET: api/Catalogos/TiposDocumento
        [HttpGet("TiposDocumento")]
        public async Task<ActionResult<IEnumerable<TipoDocumento>>> GetTiposDocumento()
        {
            return await _context.TiposDocumento.OrderBy(t => t.nombre).ToListAsync();
        }

        // GET: api/Catalogos/EstadosCiviles
        [HttpGet("EstadosCiviles")]
        public async Task<ActionResult<IEnumerable<EstadoCivil>>> GetEstadosCiviles()
        {
            return await _context.EstadosCiviles.OrderBy(e => e.nombre).ToListAsync();
        }

        // GET: api/Catalogos/Especialidades
        [HttpGet("Especialidades")]
        public async Task<ActionResult<IEnumerable<EspecialidadTerapeuta>>> GetEspecialidades()
        {
            return await _context.EspecialidadesTerapeutas.OrderBy(e => e.nombre).ToListAsync();
        }

        // --- MÉTODO AGREGADO PARA EL FORMULARIO DE TERAPIAS ---
        // GET: api/Catalogos/CategoriasPorEspecialidad/5
        [HttpGet("CategoriasPorEspecialidad/{especialidadId}")]
        public async Task<ActionResult<IEnumerable<CategoriaTerapia>>> GetCategoriasPorEspecialidad(int especialidadId)
        {
            return await _context.CategoriasTerapias
                .Where(c => c.EspecialidadId == especialidadId)
                .OrderBy(c => c.nombre)
                .ToListAsync();
        }

        // GET: api/Catalogos/EstadosCitas
        [HttpGet("EstadosCitas")]
        public async Task<ActionResult<IEnumerable<EstadoCita>>> GetEstadosCitas()
        {
            return await _context.EstadosCitas.OrderBy(e => e.nombre).ToListAsync();
        }

        // --- ENDPOINT: SERVICIOS FILTRADOS POR TERAPEUTA ---
        [HttpGet("ServiciosPorTerapeuta/{terapeutaId}")]
        public async Task<ActionResult<IEnumerable<ServicioTerapia>>> GetServiciosPorTerapeuta(int terapeutaId)
        {
            var terapeuta = await _context.Terapeutas
                .FirstOrDefaultAsync(t => t.id == terapeutaId);

            if (terapeuta == null) return NotFound("Terapeuta no encontrado");

            var servicios = await _context.Terapias
                .Include(s => s.CategoriaTerapia)
                .Where(s => s.CategoriaTerapia.EspecialidadId == terapeuta.EspecialidadTerapeutaId)
                .OrderBy(s => s.nombreServicio)
                .ToListAsync();

            return servicios;
        }
    }
}