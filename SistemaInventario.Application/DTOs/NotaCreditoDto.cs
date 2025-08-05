using System;
using System.Collections.Generic;

public class NotaCreditoDto
{
    public Guid Id { get; set; }
    public string NumeroNotaCredito { get; set; } = string.Empty;
    public Guid FacturaId { get; set; }
    public DateTime Fecha { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public List<DetalleNotaCreditoDto> Detalles { get; set; } = new();
}