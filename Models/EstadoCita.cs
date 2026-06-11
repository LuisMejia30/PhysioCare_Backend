using System.ComponentModel.DataAnnotations;

namespace physiocare_backend.Models
{
    public class EstadoCita
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string nombre { get; set; } // "Pendiente", "Realizada", etc.
    }
}
