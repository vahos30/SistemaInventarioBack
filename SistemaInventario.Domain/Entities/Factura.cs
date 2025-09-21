using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaInventario.Domain.Entities
{
    public class Factura
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string NumeroFactura { get; set; } = string.Empty; // Ej: "F501"
        public Guid ClienteId { get; set; }
        public Cliente Cliente { get; set; } = null!;
        public DateTime Fecha { get; set; } = DateTime.Now;
        public List<DetalleFactura> Detalles { get; set; } = new();
        public decimal Total { get; set; }

        // NUEVOS CAMPOS
        public bool Anulada { get; set; } = false;
        public string? MotivoAnulacion { get; set; }
        public DateTime? FechaAnulacion { get; set; }
        public Guid? NotaCreditoId { get; set; }
        public NotaCredito? NotaCredito { get; set; }
        public string FormaPago { get; set; } = string.Empty;
        public string Observacion { get; set; } = string.Empty;
        public string Referencia { get; set; } = string.Empty;
    }
}