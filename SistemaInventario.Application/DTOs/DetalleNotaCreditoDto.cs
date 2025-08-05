using System;

public class DetalleNotaCreditoDto
{
    public Guid Id { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal => Cantidad * PrecioUnitario;
    public Guid ProductoId { get; set; }
}