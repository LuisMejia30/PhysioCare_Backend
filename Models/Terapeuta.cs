using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace physiocare_backend.Models
{
    public class Terapeuta
    {
        [Key]
        public int id { get; set; }

        [Required]
        public int EspecialidadTerapeutaId { get; set; }

        [ForeignKey("EspecialidadTerapeutaId")]
        public virtual EspecialidadTerapeuta? EspecialidadTerapeuta { get; set; } // EL '?' ES VITAL

        [Required]
        public string tarjetaProfesional { get; set; }

        // Información Personal
        [Required]
        public string primerNombre { get; set; }

        public string? segundoNombre { get; set; }

        [Required]
        public string primerApellido { get; set; }

        public string? segundoApellido { get; set; }

        [Required]
        public int TipoDocumentoId { get; set; }

        [ForeignKey("TipoDocumentoId")]
        public virtual TipoDocumento? TipoDocumento { get; set; } // EL '?' ES VITAL

        [Required]
        public long numeroDocumento { get; set; }

        [Required]
        public DateTime fechaNacimiento { get; set; }

        [Required]
        public string correoLaboral { get; set; }

        [Required]
        public long telefonoContacto { get; set; }

        [Required]
        public TimeSpan horaInicio { get; set; }

        [Required]
        public TimeSpan horaFin { get; set; }

        [Required]
        public string diasLaborales { get; set; }
    }
}