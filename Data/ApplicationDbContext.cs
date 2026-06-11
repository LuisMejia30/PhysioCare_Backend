using Microsoft.EntityFrameworkCore;
using physiocare_backend.Models;

namespace physiocare_backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Terapeuta> Terapeutas { get; set; }
        public DbSet<ServicioTerapia> Terapias { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<Genero> Generos { get; set; }
        public DbSet<TipoSangre> TiposSangre { get; set; }
        public DbSet<TipoDocumento> TiposDocumento { get; set; }
        public DbSet<CategoriaTerapia> CategoriasTerapias { get; set; }
        public DbSet<EspecialidadTerapeuta> EspecialidadesTerapeutas { get; set; }
        public DbSet<EstadoCivil> EstadosCiviles { get; set; }
        public DbSet<EstadoCita> EstadosCitas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // --- ÍNDICES ---
            modelBuilder.Entity<Paciente>()
                .HasIndex(p => p.numeroDocumento)
                .IsUnique();

            modelBuilder.Entity<Terapeuta>()
                .HasIndex(t => t.numeroDocumento)
                .IsUnique();


            // Poblar Especialidades
            modelBuilder.Entity<EspecialidadTerapeuta>().HasData(
                new EspecialidadTerapeuta { id = 1, nombre = "Fisioterapia Deportiva" },
                new EspecialidadTerapeuta { id = 2, nombre = "Fisioterapia Geriátrica" },
                new EspecialidadTerapeuta { id = 3, nombre = "Fisioterapia Cardiovascular" }
            );

            // Poblar Categorías vinculadas a Especialidades (EspecialidadId = 1 es Fisioterapia)
            modelBuilder.Entity<CategoriaTerapia>().HasData(
                new CategoriaTerapia { id = 1, nombre = "Fisioterapia Preventiva (Pre-Lesión)", EspecialidadId = 1 },
                new CategoriaTerapia { id = 2, nombre = "Fisioterapia de Campo", EspecialidadId = 1 },
                new CategoriaTerapia { id = 3, nombre = "Fisioterapia de Recuperación (Post-Esfuerzo)", EspecialidadId = 1 },
                new CategoriaTerapia { id = 4, nombre = "Fisioterapia para el Envejicimiento Activo (Preventiva)", EspecialidadId = 2 },
                new CategoriaTerapia { id = 5, nombre = "Fisioterapia Neurológica Geriátrica", EspecialidadId = 2 },
                new CategoriaTerapia { id = 6, nombre = "Fisioterapia en Cuidados Paliativos", EspecialidadId = 2 },
                new CategoriaTerapia { id = 7, nombre = "Fisioterapia en Insuficiencia Cardíaca Crónica", EspecialidadId = 3 },
                new CategoriaTerapia { id = 8, nombre = "Fisioterapia Vascular Periférica", EspecialidadId = 3 }
            );

            // Poblar Géneros
            modelBuilder.Entity<Genero>().HasData(
                new Genero { id = 1, nombre = "Masculino" },
                new Genero { id = 2, nombre = "Femenino" },
                new Genero { id = 3, nombre = "Otro" }
            );

            // Poblar Tipo de Sangre
            modelBuilder.Entity<TipoSangre>().HasData(
                new TipoSangre { id = 1, nombre = "O+" },
                new TipoSangre { id = 2, nombre = "O-" },
                new TipoSangre { id = 3, nombre = "A+" },
                new TipoSangre { id = 4, nombre = "A-" },
                new TipoSangre { id = 5, nombre = "B+" },
                new TipoSangre { id = 6, nombre = "B-" },
                new TipoSangre { id = 7, nombre = "AB+" },
                new TipoSangre { id = 8, nombre = "AB-" }
            );

            // Poblar Tipo de Documento
            modelBuilder.Entity<TipoDocumento>().HasData(
                new TipoDocumento { id = 1, nombre = "Cédula de Ciudadanía" },
                new TipoDocumento { id = 2, nombre = "Tarjeta de Identidad" },
                new TipoDocumento { id = 3, nombre = "Cédula de Extranjería" },
                new TipoDocumento { id = 4, nombre = "Pasaporte" }
            );

            // Poblar Estado Civil
            modelBuilder.Entity<EstadoCivil>().HasData(
                new EstadoCivil { id = 1, nombre = "Soltero(a)" },
                new EstadoCivil { id = 2, nombre = "Casado(a)" },
                new EstadoCivil { id = 3, nombre = "Unión Libre" },
                new EstadoCivil { id = 4, nombre = "Divorciado(a)" },
                new EstadoCivil { id = 5, nombre = "Viudo(a)" }
            );

            // Poblar Estados de Cita
            modelBuilder.Entity<EstadoCita>().HasData(
                new EstadoCita { id = 1, nombre = "Programada" },
                new EstadoCita { id = 2, nombre = "Confirmada" },
                new EstadoCita { id = 3, nombre = "En Curso" },
                new EstadoCita { id = 4, nombre = "Completada" },
                new EstadoCita { id = 5, nombre = "Cancelada" }
            );

            // Nombre de usuario: Admin
            // Contraseña limpia: Admin1234
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    id = 1,
                    username = "Admin",
                    // Este hash largo representa la contraseña 'Admin1234' encriptada con BCrypt
                    passwordHash = "$2a$11$cD0U2F8C0x9jDYahMGtr1O521T8ekJN/uVKTpgijHBFjl/ODn1wC6"
                }
            );
        }
    }
}