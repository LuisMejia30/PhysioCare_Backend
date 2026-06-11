using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace physiocare_backend.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarHashAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id",
                keyValue: 1,
                column: "passwordHash",
                value: "$2a$11$cD0U2F8C0x9jDYahMGtr1O521T8ekJN/uVKTpgijHBFjl/ODn1wC6");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id",
                keyValue: 1,
                column: "passwordHash",
                value: "$2a$12$ZpYbeSWhvLg7l6Z5GskqXOnlPCOvF80ZJc9bVvF0XN6h3u6bBfTpq");
        }
    }
}
