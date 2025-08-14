using System.Collections.Generic;

namespace SistemaInventario.Application.DTOs
{
    public class VentasDiariasDto
    {
        public List<ReciboDto> Recibos { get; set; } = new();
        public List<FacturaDto> Facturas { get; set; } = new();
    }
}