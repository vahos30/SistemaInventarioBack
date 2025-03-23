using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SistemaInventario.Application.DTOs;

namespace SistemaInventario.Application.Feactures.Recibos
{
    /// <summary>
    /// Consulta para obtener los recibos de un cliente.
    /// </summary>
    public record ObtenerRecibosPorClienteQuery(Guid ClienteId) : IRequest<IEnumerable<ReciboDto>>;



}
