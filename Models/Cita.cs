using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace physiocare_backend.Models
{
    public class Cita
    {
        [Key]
        public int id { get; set; }

        // Referencia al Paciente
        [Required(ErrorMessage = "El paciente es obligatorio")]
        public int PacienteId { get; set; }

        [ForeignKey("PacienteId")]
        public virtual Paciente? Paciente { get; set; }

        // Referencia al Terapeuta
        [Required(ErrorMessage = "El terapeuta es obligatorio")]
        public int TerapeutaId { get; set; }

        [ForeignKey("TerapeutaId")]
        public virtual Terapeuta? Terapeuta { get; set; }

        // Única referencia al Servicio (Desde aquí el sistema sabe la categoría y especialidad)
        [Required(ErrorMessage = "El servicio es obligatorio")]
        public int ServicioTerapiaId { get; set; }

        [ForeignKey("ServicioTerapiaId")]
        public virtual ServicioTerapia? ServicioTerapia { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime fecha { get; set; }

        [Required(ErrorMessage = "La hora de inicio es obligatoria")]
        public TimeSpan horaInicio { get; set; }

        [Required]
        public int duracionEstimada { get; set; } // En minutos

        [Required]
        public int EstadoCitaId { get; set; }

        [ForeignKey("EstadoCitaId")]
        public virtual EstadoCita? EstadoCita { get; set; }

        public string? notas { get; set; }
    }
}