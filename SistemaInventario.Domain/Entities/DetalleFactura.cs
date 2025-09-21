using System;

namespace SistemaInventario.Domain.Entities
{
    public class DetalleFactura
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid FacturaId { get; set; }
        public Factura? Factura { get; set; }
        public Guid ProductoId { get; set; }
        public Producto Producto { get; set; } = null!;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public string? TipoDescuento { get; set; }
        public decimal? ValorDescuento { get; set; }
        public decimal ValorIva { get; set; }
        public decimal Subtotal { get; set; }
    }
}