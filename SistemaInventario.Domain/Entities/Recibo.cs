using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Domain.Entities
{
    // Representa un recibo de venta generada para un cliente
    public class Recibo
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // se asocia al cliente mediante su Id.
        public Guid ClienteId { get; set; }

        // cliente completo para navegacion
        public Cliente Cliente { get; set; } = null!;

        // fecha en la que se realiza la factura
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        // lista de los detalles del recibo

        public List<DetalleRecibo> Detalles { get; set; } = new();
    }
}
