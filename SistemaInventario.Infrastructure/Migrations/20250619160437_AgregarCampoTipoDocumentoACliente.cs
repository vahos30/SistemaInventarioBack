using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaInventario.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCampoTipoDocumentoACliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TipoDocumento",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoDocumento",
                table: "Clientes");
        }
    }
}
