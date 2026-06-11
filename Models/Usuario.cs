using System.ComponentModel.DataAnnotations;

namespace physiocare_backend.Models
{
    public class Usuario
    {
        [Key]
        public int id { get; set; }

        [Required]
        [StringLength(50)]
        public string username { get; set; }

        [Required]
        public string passwordHash { get; set; }
    }
}
