using System;
using SistemaInventario.Domain.Entities;

public class DetalleNotaCredito
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid NotaCreditoId { get; set; }
    public NotaCredito NotaCredito { get; set; } = null!;
    public Guid ProductoId { get; set; }
    public Producto Producto { get; set; } = null!;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal => Cantidad * PrecioUnitario;
}