using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaInventario.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarReferenciaAProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Referencia",
                table: "Productos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Referencia",
                table: "Productos");
        }
    }
}
