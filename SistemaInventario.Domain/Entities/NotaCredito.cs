using System;
using System.Collections.Generic;
using SistemaInventario.Domain.Entities;

public class NotaCredito
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string NumeroNotaCredito { get; set; } = string.Empty;
    public Guid FacturaId { get; set; }
    public Factura Factura { get; set; } = null!;
    public DateTime Fecha { get; set; } = DateTime.Now;
    public string Motivo { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public List<DetalleNotaCredito> Detalles { get; set; } = new();
}