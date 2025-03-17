using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SistemaInventario.Application.DTOs;

namespace SistemaInventario.Application.Feactures.Reportes
/// <summary>
/// QUERY para obtener las ventas diarias (Recibos).
{
    public record ObtenerVentasDiariasQuery() : IRequest<IEnumerable<ReciboDto>>;


}
