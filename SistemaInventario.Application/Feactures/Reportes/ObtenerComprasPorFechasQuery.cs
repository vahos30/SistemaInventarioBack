using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SistemaInventario.Application.DTOs;

namespace SistemaInventario.Application.Feactures.Reportes
{
    public class ObtenerComprasPorFechasQuery : IRequest<List<CompraReporteDto>>
    {
        public DateTime? FechaInicio { get; }
        public DateTime? FechaFin { get; }

        public ObtenerComprasPorFechasQuery(DateTime? fechaInicio, DateTime? fechaFin)
        {
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
        }
    }
}
