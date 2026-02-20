using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaterPonto.Migrations
{
    /// <inheritdoc />
    public partial class AddUserToRegistro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "RegistrosPonto",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosPonto_UserId",
                table: "RegistrosPonto",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosPonto_AspNetUsers_UserId",
                table: "RegistrosPonto",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosPonto_AspNetUsers_UserId",
                table: "RegistrosPonto");

            migrationBuilder.DropIndex(
                name: "IX_RegistrosPonto_UserId",
                table: "RegistrosPonto");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "RegistrosPonto");
        }
    }
}
