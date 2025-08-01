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

        // Tipo de descuento: "Porcentaje" o "ValorAbsoluto"
        public string? TipoDescuento { get; set; }

        // Valor del descuento (porcentaje o valor absoluto)
        public decimal? ValorDescuento { get; set; }

        // Valor del IVA aplicado al producto (puede ser 0 si no aplica)
        public decimal ValorIva { get; set; }

        // Subtotal con descuento aplicado
        public decimal Subtotal
        {
            get
            {
                decimal descuento = 0;
                if (TipoDescuento == "Porcentaje" && ValorDescuento.HasValue)
                {
                    descuento = PrecioUnitario * (ValorDescuento.Value / 100m);
                }
                else if (TipoDescuento == "ValorAbsoluto" && ValorDescuento.HasValue)
                {
                    descuento = ValorDescuento.Value;
                }
                decimal precioFinal = PrecioUnitario - descuento;
                if (precioFinal < 0) precioFinal = 0;
                return Cantidad * precioFinal;
            }
        }
    }
}
