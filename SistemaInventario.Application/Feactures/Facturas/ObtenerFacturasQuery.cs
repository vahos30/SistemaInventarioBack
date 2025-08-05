using MediatR;
using SistemaInventario.Application.DTOs;
using System.Collections.Generic;

public class ObtenerFacturasQuery : IRequest<List<FacturaDto>>
{
}