using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaInventario.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFormaPagoToRecibo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FormaPago",
                table: "Recibos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormaPago",
                table: "Recibos");
        }
    }
}
