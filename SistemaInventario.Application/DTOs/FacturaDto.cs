using System;
using System.Collections.Generic;

namespace SistemaInventario.Application.DTOs
{
    public class FacturaDto
    {
        public Guid Id { get; set; }
        public string NumeroFactura { get; set; } = string.Empty;
        public Guid ClienteId { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public List<DetalleFacturaDto> Detalles { get; set; } = new();
    }
}