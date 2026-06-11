using System.ComponentModel.DataAnnotations;

namespace physiocare_backend.Models
{
    public class TipoSangre
    {
        [Key]
        public int id { get; set; }

        [Required]
        [StringLength(3)]
        public string nombre { get; set; }
    }
}
