using MediatR;
using SistemaInventario.Application.DTOs;
using System;
using System.Collections.Generic;

public class CrearFacturaCommand : IRequest<FacturaDto>
{
    public Guid ClienteId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public List<DetalleFacturaDto> Detalles { get; set; } = new();
    public string FormaPago { get; set; } = string.Empty;
}