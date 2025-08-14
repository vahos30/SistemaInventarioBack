using MediatR;
using SistemaInventario.Application.DTOs;
using System;

public class ObtenerVentasPorClienteQuery : IRequest<VentasPorClienteDto>
{
    public Guid ClienteId { get; }
    public ObtenerVentasPorClienteQuery(Guid clienteId)
    {
        ClienteId = clienteId;
    }
}