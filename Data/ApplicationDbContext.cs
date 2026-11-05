using System;
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
        
        // --- DbSet para Sesiones de Terapia ---
        public DbSet<SesionTerapia> SesionesTerapia { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Evitar eliminaciones en cascada de forma global
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // --- Relaciones de Usuario para vincular cuentas con entidades (Opcionales) ---
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Paciente)
                .WithMany()
                .HasForeignKey(u => u.PacienteId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Terapeuta)
                .WithMany()
                .HasForeignKey(u => u.TerapeutaId)
                .OnDelete(DeleteBehavior.SetNull);

            // --- Relaciones explícitas de SesionTerapia ---
            modelBuilder.Entity<SesionTerapia>()
                .HasOne(s => s.Cita)
                .WithMany()
                .HasForeignKey(s => s.CitaId);

            modelBuilder.Entity<SesionTerapia>()
                .HasOne(s => s.Paciente)
                .WithMany()
                .HasForeignKey(s => s.PacienteId);

            modelBuilder.Entity<SesionTerapia>()
                .HasOne(s => s.Terapeuta)
                .WithMany()
                .HasForeignKey(s => s.TerapeutaId);

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

            // --- NUEVO: Semillar Entidades Base obligatorias para las llaves foráneas ---
            modelBuilder.Entity<Paciente>().HasData(
                new Paciente
                {
                    id = 1,
                    primerNombre = "Paciente",
                    primerApellido = "Inicial",
                    numeroDocumento = 1000000001,
                    telefono = 3000000000,
                    correoElectronico = "paciente1@physiocare.com",
                    fechaNacimiento = new DateTime(1990, 1, 1),
                    TipoDocumentoId = 1,
                    GeneroId = 1,
                    TipoSangreId = 1,
                    EstadoCivilId = 1
                }
            );

            modelBuilder.Entity<Terapeuta>().HasData(
                new Terapeuta
                {
                    id = 1,
                    tarjetaProfesional = "TP-12345",
                    primerNombre = "Terapeuta",
                    primerApellido = "Inicial",
                    numeroDocumento = 2000000002,
                    fechaNacimiento = new DateTime(1985, 5, 15),
                    correoLaboral = "terapeuta1@physiocare.com",
                    telefonoContacto = 3000000001,
                    horaInicio = new TimeSpan(8, 0, 0),
                    horaFin = new TimeSpan(17, 0, 0),
                    diasLaborales = "Lunes a Viernes",
                    TipoDocumentoId = 1,
                    EspecialidadTerapeutaId = 1
                }
            );

            // --- Poblar Usuarios de Prueba vinculados ---
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    id = 1,
                    username = "Admin",
                    passwordHash = "$2a$11$cD0U2F8C0x9jDYahMGtr1O521T8ekJN/uVKTpgijHBFjl/ODn1wC6",
                    rol = "Administrador", 
                    activo = true
                },
                new Usuario
                {
                    id = 2,
                    username = "recepcion",
                    passwordHash = "$2a$11$cD0U2F8C0x9jDYahMGtr1O521T8ekJN/uVKTpgijHBFjl/ODn1wC6",
                    rol = "Recepcionista",
                    activo = true
                },
                new Usuario
                {
                    id = 3,
                    username = "terapeuta1",
                    passwordHash = "$2a$11$cD0U2F8C0x9jDYahMGtr1O521T8ekJN/uVKTpgijHBFjl/ODn1wC6",
                    rol = "Terapeuta",
                    TerapeutaId = 1,
                    activo = true
                },
                new Usuario
                {
                    id = 4,
                    username = "paciente1",
                    passwordHash = "$2a$11$cD0U2F8C0x9jDYahMGtr1O521T8ekJN/uVKTpgijHBFjl/ODn1wC6",
                    rol = "Paciente",
                    PacienteId = 1,
                    activo = true
                }
            );
        }
    }
}