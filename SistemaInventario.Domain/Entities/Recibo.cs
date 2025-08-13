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

        // Se asocia al cliente mediante su Id.
        public Guid ClienteId { get; set; }

        // Cliente completo para navegación.
        public Cliente Cliente { get; set; } = null!;

        // Fecha en la que se realiza la factura.
        public DateTime Fecha { get; set; } = DateTime.Now;

        // Lista de los detalles del recibo.
        public List<DetalleRecibo> Detalles { get; set; } = new();

        // Total con descuentos aplicados
        public decimal Total => Detalles.Sum(d => d.Subtotal);

        // Forma de pago utilizada en el recibo.
        public string FormaPago { get; set; } = string.Empty;
    }
}

