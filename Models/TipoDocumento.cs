using System.ComponentModel.DataAnnotations;

namespace physiocare_backend.Models
{
    public class TipoDocumento
    {
        [Key]
        public int id { get; set; }

        [Required]
        [StringLength(30)]
        public string nombre { get; set; }
    }
}
