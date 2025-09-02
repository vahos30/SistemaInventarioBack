using System;

namespace SistemaInventario.Domain.Entities
{
    public class DetalleCompra
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CompraId { get; set; }
        public Compra Compra { get; set; }
        public Guid ProductoId { get; set; }
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal SubTotal { get; set; }
        public string? MotivoDevolucion { get; set; } // <-- NUEVO
    }
}

