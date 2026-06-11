using System.ComponentModel.DataAnnotations;

namespace physiocare_backend.Models
{
    public class EspecialidadTerapeuta
    {
        [Key]
        public int id { get; set; }

        [Required]
        [StringLength(100)]
        public string nombre { get; set; }
    }
}
