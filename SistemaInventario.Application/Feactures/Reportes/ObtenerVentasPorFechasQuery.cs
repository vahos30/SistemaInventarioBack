using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SistemaInventario.Application.DTOs;

namespace SistemaInventario.Application.Feactures.Reportes
{
    public record ObtenerVentasPorFechasQuery(DateTime FechaInicio, DateTime FechaFin): IRequest<IEnumerable<ReciboDto>>;


}
