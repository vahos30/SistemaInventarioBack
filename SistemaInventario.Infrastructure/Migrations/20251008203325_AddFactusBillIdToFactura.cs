using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaInventario.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFactusBillIdToFactura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FactusBillId",
                table: "Facturas",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FactusBillId",
                table: "Facturas");
        }
    }
}
