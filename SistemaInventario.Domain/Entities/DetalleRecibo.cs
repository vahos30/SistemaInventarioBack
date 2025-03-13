using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Domain.Entities
{
    // Representa el detalle de un prodcuto en un recibo
    public class DetalleRecibo
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Relacion con el recibo
        public Guid ReciboId { get; set; }
        public Recibo Recibo { get; set; } = null!;

        // Relacion con el producto
        public Guid ProductoId { get; set; }
        public Producto Producto { get; set; } = null!;

        // Cantidad de productos en el recibo
        public int Cantidad { get; set; }

        // Precio unitario del producto al momento de la venta
        public decimal PrecioUnitario { get; set; }

    }
}
