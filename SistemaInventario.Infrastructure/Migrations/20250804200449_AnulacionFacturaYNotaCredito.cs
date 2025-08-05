using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaInventario.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AnulacionFacturaYNotaCredito : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Anulada",
                table: "Facturas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAnulacion",
                table: "Facturas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoAnulacion",
                table: "Facturas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NotaCreditoId",
                table: "Facturas",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NotasCredito",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumeroNotaCredito = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FacturaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotasCredito", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotasCredito_Facturas_FacturaId",
                        column: x => x.FacturaId,
                        principalTable: "Facturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetallesNotaCredito",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotaCreditoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesNotaCredito", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesNotaCredito_NotasCredito_NotaCreditoId",
                        column: x => x.NotaCreditoId,
                        principalTable: "NotasCredito",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesNotaCredito_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetallesNotaCredito_NotaCreditoId",
                table: "DetallesNotaCredito",
                column: "NotaCreditoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesNotaCredito_ProductoId",
                table: "DetallesNotaCredito",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_NotasCredito_FacturaId",
                table: "NotasCredito",
                column: "FacturaId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetallesNotaCredito");

            migrationBuilder.DropTable(
                name: "NotasCredito");

            migrationBuilder.DropColumn(
                name: "Anulada",
                table: "Facturas");

            migrationBuilder.DropColumn(
                name: "FechaAnulacion",
                table: "Facturas");

            migrationBuilder.DropColumn(
                name: "MotivoAnulacion",
                table: "Facturas");

            migrationBuilder.DropColumn(
                name: "NotaCreditoId",
                table: "Facturas");
        }
    }
}
