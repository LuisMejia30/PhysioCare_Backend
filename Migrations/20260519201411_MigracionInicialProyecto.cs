using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace physiocare_backend.Migrations
{
    /// <inheritdoc />
    public partial class MigracionInicialProyecto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EspecialidadesTerapeutas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EspecialidadesTerapeutas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "EstadosCitas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosCitas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "EstadosCiviles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosCiviles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Generos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Generos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "TiposDocumento",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposDocumento", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "TiposSangre",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposSangre", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    passwordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "CategoriasTerapias",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    EspecialidadId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasTerapias", x => x.id);
                    table.ForeignKey(
                        name: "FK_CategoriasTerapias_EspecialidadesTerapeutas_EspecialidadId",
                        column: x => x.EspecialidadId,
                        principalTable: "EspecialidadesTerapeutas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Terapeutas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EspecialidadTerapeutaId = table.Column<int>(type: "int", nullable: false),
                    tarjetaProfesional = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    primerNombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    segundoNombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    primerApellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    segundoApellido = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoDocumentoId = table.Column<int>(type: "int", nullable: false),
                    numeroDocumento = table.Column<long>(type: "bigint", nullable: false),
                    fechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    correoLaboral = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    telefonoContacto = table.Column<long>(type: "bigint", nullable: false),
                    horaInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    horaFin = table.Column<TimeSpan>(type: "time", nullable: false),
                    diasLaborales = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terapeutas", x => x.id);
                    table.ForeignKey(
                        name: "FK_Terapeutas_EspecialidadesTerapeutas_EspecialidadTerapeutaId",
                        column: x => x.EspecialidadTerapeutaId,
                        principalTable: "EspecialidadesTerapeutas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Terapeutas_TiposDocumento_TipoDocumentoId",
                        column: x => x.TipoDocumentoId,
                        principalTable: "TiposDocumento",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pacientes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoDocumentoId = table.Column<int>(type: "int", nullable: false),
                    numeroDocumento = table.Column<long>(type: "bigint", nullable: false),
                    GeneroId = table.Column<int>(type: "int", nullable: false),
                    primerNombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    segundoNombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    primerApellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    segundoApellido = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoSangreId = table.Column<int>(type: "int", nullable: false),
                    fechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstadoCivilId = table.Column<int>(type: "int", nullable: false),
                    telefono = table.Column<long>(type: "bigint", nullable: false),
                    correoElectronico = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    direccionResidencia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    telefonoEmergencia = table.Column<long>(type: "bigint", nullable: true),
                    resumenClinico = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pacientes", x => x.id);
                    table.ForeignKey(
                        name: "FK_Pacientes_EstadosCiviles_EstadoCivilId",
                        column: x => x.EstadoCivilId,
                        principalTable: "EstadosCiviles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pacientes_Generos_GeneroId",
                        column: x => x.GeneroId,
                        principalTable: "Generos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pacientes_TiposDocumento_TipoDocumentoId",
                        column: x => x.TipoDocumentoId,
                        principalTable: "TiposDocumento",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pacientes_TiposSangre_TipoSangreId",
                        column: x => x.TipoSangreId,
                        principalTable: "TiposSangre",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Terapias",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombreServicio = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CategoriaTerapiaId = table.Column<int>(type: "int", nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terapias", x => x.id);
                    table.ForeignKey(
                        name: "FK_Terapias_CategoriasTerapias_CategoriaTerapiaId",
                        column: x => x.CategoriaTerapiaId,
                        principalTable: "CategoriasTerapias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Citas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PacienteId = table.Column<int>(type: "int", nullable: false),
                    TerapeutaId = table.Column<int>(type: "int", nullable: false),
                    ServicioTerapiaId = table.Column<int>(type: "int", nullable: false),
                    fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    horaInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    duracionEstimada = table.Column<int>(type: "int", nullable: false),
                    EstadoCitaId = table.Column<int>(type: "int", nullable: false),
                    notas = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Citas", x => x.id);
                    table.ForeignKey(
                        name: "FK_Citas_EstadosCitas_EstadoCitaId",
                        column: x => x.EstadoCitaId,
                        principalTable: "EstadosCitas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Citas_Pacientes_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Pacientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Citas_Terapeutas_TerapeutaId",
                        column: x => x.TerapeutaId,
                        principalTable: "Terapeutas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Citas_Terapias_ServicioTerapiaId",
                        column: x => x.ServicioTerapiaId,
                        principalTable: "Terapias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "EspecialidadesTerapeutas",
                columns: new[] { "id", "nombre" },
                values: new object[,]
                {
                    { 1, "Fisioterapia Deportiva" },
                    { 2, "Fisioterapia Geriátrica" },
                    { 3, "Fisioterapia Cardiovascular" }
                });

            migrationBuilder.InsertData(
                table: "EstadosCitas",
                columns: new[] { "id", "nombre" },
                values: new object[,]
                {
                    { 1, "Programada" },
                    { 2, "Confirmada" },
                    { 3, "En Curso" },
                    { 4, "Completada" },
                    { 5, "Cancelada" }
                });

            migrationBuilder.InsertData(
                table: "EstadosCiviles",
                columns: new[] { "id", "nombre" },
                values: new object[,]
                {
                    { 1, "Soltero(a)" },
                    { 2, "Casado(a)" },
                    { 3, "Unión Libre" },
                    { 4, "Divorciado(a)" },
                    { 5, "Viudo(a)" }
                });

            migrationBuilder.InsertData(
                table: "Generos",
                columns: new[] { "id", "nombre" },
                values: new object[,]
                {
                    { 1, "Masculino" },
                    { 2, "Femenino" },
                    { 3, "Otro" }
                });

            migrationBuilder.InsertData(
                table: "TiposDocumento",
                columns: new[] { "id", "nombre" },
                values: new object[,]
                {
                    { 1, "Cédula de Ciudadanía" },
                    { 2, "Tarjeta de Identidad" },
                    { 3, "Cédula de Extranjería" },
                    { 4, "Pasaporte" }
                });

            migrationBuilder.InsertData(
                table: "TiposSangre",
                columns: new[] { "id", "nombre" },
                values: new object[,]
                {
                    { 1, "O+" },
                    { 2, "O-" },
                    { 3, "A+" },
                    { 4, "A-" },
                    { 5, "B+" },
                    { 6, "B-" },
                    { 7, "AB+" },
                    { 8, "AB-" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "id", "passwordHash", "username" },
                values: new object[] { 1, "$2a$12$ZpYbeSWhvLg7l6Z5GskqXOnlPCOvF80ZJc9bVvF0XN6h3u6bBfTpq", "Admin" });

            migrationBuilder.InsertData(
                table: "CategoriasTerapias",
                columns: new[] { "id", "EspecialidadId", "nombre" },
                values: new object[,]
                {
                    { 1, 1, "Fisioterapia Preventiva (Pre-Lesión)" },
                    { 2, 1, "Fisioterapia de Campo" },
                    { 3, 1, "Fisioterapia de Recuperación (Post-Esfuerzo)" },
                    { 4, 2, "Fisioterapia para el Envejicimiento Activo (Preventiva)" },
                    { 5, 2, "Fisioterapia Neurológica Geriátrica" },
                    { 6, 2, "Fisioterapia en Cuidados Paliativos" },
                    { 7, 3, "Fisioterapia en Insuficiencia Cardíaca Crónica" },
                    { 8, 3, "Fisioterapia Vascular Periférica" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoriasTerapias_EspecialidadId",
                table: "CategoriasTerapias",
                column: "EspecialidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_EstadoCitaId",
                table: "Citas",
                column: "EstadoCitaId");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_PacienteId",
                table: "Citas",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_ServicioTerapiaId",
                table: "Citas",
                column: "ServicioTerapiaId");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_TerapeutaId",
                table: "Citas",
                column: "TerapeutaId");

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_EstadoCivilId",
                table: "Pacientes",
                column: "EstadoCivilId");

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_GeneroId",
                table: "Pacientes",
                column: "GeneroId");

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_numeroDocumento",
                table: "Pacientes",
                column: "numeroDocumento",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_TipoDocumentoId",
                table: "Pacientes",
                column: "TipoDocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_TipoSangreId",
                table: "Pacientes",
                column: "TipoSangreId");

            migrationBuilder.CreateIndex(
                name: "IX_Terapeutas_EspecialidadTerapeutaId",
                table: "Terapeutas",
                column: "EspecialidadTerapeutaId");

            migrationBuilder.CreateIndex(
                name: "IX_Terapeutas_numeroDocumento",
                table: "Terapeutas",
                column: "numeroDocumento",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Terapeutas_TipoDocumentoId",
                table: "Terapeutas",
                column: "TipoDocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Terapias_CategoriaTerapiaId",
                table: "Terapias",
                column: "CategoriaTerapiaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Citas");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "EstadosCitas");

            migrationBuilder.DropTable(
                name: "Pacientes");

            migrationBuilder.DropTable(
                name: "Terapeutas");

            migrationBuilder.DropTable(
                name: "Terapias");

            migrationBuilder.DropTable(
                name: "EstadosCiviles");

            migrationBuilder.DropTable(
                name: "Generos");

            migrationBuilder.DropTable(
                name: "TiposSangre");

            migrationBuilder.DropTable(
                name: "TiposDocumento");

            migrationBuilder.DropTable(
                name: "CategoriasTerapias");

            migrationBuilder.DropTable(
                name: "EspecialidadesTerapeutas");
        }
    }
}
