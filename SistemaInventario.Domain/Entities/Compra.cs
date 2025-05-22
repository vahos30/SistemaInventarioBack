using System;
using System.Collections.Generic;

namespace SistemaInventario.Domain.Entities
{
    public class Compra
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProveedorId { get; set; }
        public Proveedor Proveedor { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public decimal Total { get; set; }
        public ICollection<DetalleCompra> Detalles { get; set; } = new List<DetalleCompra>();
    }
}
