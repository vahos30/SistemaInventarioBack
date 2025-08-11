using System;

namespace SistemaInventario.Application.DTOs
{
    public class FacturaAnuladaDto
    {
        public string NumeroFactura { get; set; } = string.Empty;
        public string? NumeroNotaCredito { get; set; }
        public string? MotivoAnulacion { get; set; }
        public DateTime? FechaAnulacion { get; set; }
        public decimal Total { get; set; }
    }
}