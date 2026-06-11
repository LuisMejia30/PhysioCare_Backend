using System.ComponentModel.DataAnnotations;

namespace physiocare_backend.Models
{
    public class Genero
    {
        [Key]
        public int id { get; set; }

        [Required]
        [StringLength(10)]
        public string nombre { get; set; }
    }
}
