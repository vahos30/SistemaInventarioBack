using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SistemaInventario.Application.DTOs;

namespace SistemaInventario.Application.Feactures.Recibos

/// <summary>
/// Query para obtener todos los recibos
{
    public record ObtenerRecibosQuery() : IRequest<List<ReciboDto>>
    {
    }
}
