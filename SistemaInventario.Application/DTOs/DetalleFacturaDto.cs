using System;

namespace SistemaInventario.Application.DTOs
{
    public class DetalleFacturaDto
    {
        public Guid Id { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public string? TipoDescuento { get; set; }
        public decimal? ValorDescuento { get; set; }
        public decimal ValorIva { get; set; }
        public decimal Subtotal
        {
            get
            {
                decimal descuento = 0;
                if (TipoDescuento == "Porcentaje" && ValorDescuento.HasValue)
                    descuento = PrecioUnitario * (ValorDescuento.Value / 100m);
                else if (TipoDescuento == "ValorAbsoluto" && ValorDescuento.HasValue)
                    descuento = ValorDescuento.Value;

                decimal precioFinal = PrecioUnitario - descuento;
                if (precioFinal < 0) precioFinal = 0;
                return Cantidad * precioFinal;
            }
        }
        public Guid ProductoId { get; set; }
    }
}