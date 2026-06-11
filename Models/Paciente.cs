using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace physiocare_backend.Models
{
    public partial class Paciente
    {
        [Key]
        public int id { get; set; }

        // Datos de Identificación
        [Required]
        public int TipoDocumentoId { get; set; }

        [ForeignKey("TipoDocumentoId")]
        // Agregamos '?' para que sea opcional en la validación del JSON de entrada
        public virtual TipoDocumento? TipoDocumento { get; set; }

        [Required]
        public long numeroDocumento { get; set; }

        [Required]
        public int GeneroId { get; set; }

        [ForeignKey("GeneroId")]
        public virtual Genero? Genero { get; set; }

        // Nombres y Apellidos
        [Required]
        public string primerNombre { get; set; }

        public string? segundoNombre { get; set; }

        [Required]
        public string primerApellido { get; set; }

        public string? segundoApellido { get; set; }

        // Información Personal y Médica
        [Required]
        public int TipoSangreId { get; set; }

        [ForeignKey("TipoSangreId")]
        public virtual TipoSangre? TipoSangre { get; set; }

        [Required]
        public DateTime fechaNacimiento { get; set; }

        [Required]
        public int EstadoCivilId { get; set; }

        [ForeignKey("EstadoCivilId")]
        public virtual EstadoCivil? EstadoCivil { get; set; }

        [Required]
        public long telefono { get; set; }

        // Contacto y Localización
        public string? correoElectronico { get; set; }
        public string? direccionResidencia { get; set; }
        public long? telefonoEmergencia { get; set; }
        public string? resumenClinico { get; set; }
    }
}