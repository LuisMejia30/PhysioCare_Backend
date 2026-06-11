using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace physiocare_backend.Models
{
    public class CategoriaTerapia
    {
        [Key]
        public int id { get; set; }

        [Required]
        [StringLength(150)]
        public string nombre { get; set; }

        // Nueva relación: Una categoría pertenece a una especialidad
        [Required]
        public int EspecialidadId { get; set; }

        [ForeignKey("EspecialidadId")]
        public virtual EspecialidadTerapeuta? Especialidad { get; set; }
    }
}