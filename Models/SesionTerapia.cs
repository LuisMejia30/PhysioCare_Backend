namespace physiocare_backend.Models
{
    public class SesionTerapia
    {
        public int id { get; set; }
        
        public int CitaId { get; set; }
        public Cita Cita { get; set; } = null!;
        
        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; } = null!;
        
        public int TerapeutaId { get; set; }
        public Terapeuta Terapeuta { get; set; } = null!;

        public DateTime fechaSesion { get; set; }
        public string? observaciones { get; set; }
        public string? tratamientoRealizado { get; set; }
        public string? evolucion { get; set; }
        public string? recomendaciones { get; set; }
        public string estado { get; set; } = "Programada"; // Programada, Completada, Cancelada
    }
}