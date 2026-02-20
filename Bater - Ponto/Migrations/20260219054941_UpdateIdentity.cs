using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bater___Ponto.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Registros",
                table: "Registros");

            migrationBuilder.RenameTable(
                name: "Registros",
                newName: "RegistrosPonto");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RegistrosPonto",
                table: "RegistrosPonto",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RegistrosPonto",
                table: "RegistrosPonto");

            migrationBuilder.RenameTable(
                name: "RegistrosPonto",
                newName: "Registros");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Registros",
                table: "Registros",
                column: "Id");
        }
    }
}
