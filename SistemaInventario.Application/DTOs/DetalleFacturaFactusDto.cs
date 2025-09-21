using System;

namespace SistemaInventario.Application.DTOs
{
    public class DetalleFacturaFactusDto
    {
        public Guid ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public string? TipoDescuento { get; set; }
        public decimal? ValorDescuento { get; set; }
    }
}
