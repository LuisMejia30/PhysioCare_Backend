using System.Text.Json.Serialization;

namespace physiocare_backend.Models
{
    public class Usuario
    {
        public int id { get; set; }
        public string username { get; set; } = null!;
        public string passwordHash { get; set; } = null!;
        public string rol { get; set; } = "Paciente"; // Administrador, Recepcionista, Terapeuta, Paciente
        public bool activo { get; set; } = true;

        // Relaciones opcionales (Para vincular cuentas con entidades físicas)
        public int? PacienteId { get; set; }
        [JsonIgnore]
        public Paciente? Paciente { get; set; }

        public int? TerapeutaId { get; set; }
        [JsonIgnore]
        public Terapeuta? Terapeuta { get; set; }
    }
}