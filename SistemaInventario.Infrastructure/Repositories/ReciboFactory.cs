using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaInventario.Domain.Entities;

namespace SistemaInventario.Infrastructure.Repositories
{
    /// <summary>
    /// Fábrica para crear facturas con sus detalles.
    /// </summary>
    public static class ReciboFactory
    {
        /// <summary>
        /// Crea una recibo para un cliente, generando los detalles de la factura.
        /// </summary>
        /// <param name="clienteId">Identificador del cliente.</param>
        /// <param name="detalles">
        /// Lista de tuplas que contienen: (productoId, cantidad, precio unitario).
        /// </param>
        /// <returns>Una nueva instancia de Factura.</returns>
        public static Recibo CrearRecibo(Guid clienteId, List<(Guid productoId, int cantidad, decimal precio)> detalles)
        {
            var recibo = new Recibo
            {
                ClienteId = clienteId,
                Fecha = DateTime.UtcNow,
                Detalles = new List<DetalleRecibo>()
            };

            // Agregar cada detalle a la factura
            foreach (var detalle in detalles)
            {
                recibo.Detalles.Add(new DetalleRecibo
                {
                    ProductoId = detalle.productoId,
                    Cantidad = detalle.cantidad,
                    PrecioUnitario = detalle.precio
                });
            }

            return recibo;
        }
    }
}
