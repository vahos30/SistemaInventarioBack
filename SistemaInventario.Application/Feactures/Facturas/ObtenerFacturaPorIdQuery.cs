using MediatR;
using SistemaInventario.Application.DTOs;
using System;

public class ObtenerFacturaPorIdQuery : IRequest<FacturaDto>
{
    public Guid Id { get; set; }
    public ObtenerFacturaPorIdQuery(Guid id) => Id = id;
}