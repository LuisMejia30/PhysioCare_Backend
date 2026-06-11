using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace physiocare_backend.Models
{
    public class ServicioTerapia
    {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "El nombre del servicio es obligatorio")]
        [StringLength(100)]
        public string nombreServicio { get; set; }

        // Relación con la categoría (que a su vez tiene la especialidad)
        [Required(ErrorMessage = "La categoría es obligatoria")]
        public int CategoriaTerapiaId { get; set; }

        [ForeignKey("CategoriaTerapiaId")]
        public virtual CategoriaTerapia? CategoriaTerapia { get; set; }

        [StringLength(500)]
        public string? descripcion { get; set; }
    }
}