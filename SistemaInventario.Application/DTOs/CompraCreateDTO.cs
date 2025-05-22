using System;
using System.Collections.Generic;

public class CompraCreateDto
{
    public Guid ProveedorId { get; set; }
    public List<CompraDetalleCreateDto> Detalles { get; set; }
}

public class CompraDetalleCreateDto
{
    public Guid ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}
