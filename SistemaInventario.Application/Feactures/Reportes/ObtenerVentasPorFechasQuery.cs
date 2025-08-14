using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SistemaInventario.Application.DTOs;

namespace SistemaInventario.Application.Feactures.Reportes
{
    public class ObtenerVentasPorFechasQuery : IRequest<VentasPorFechasDto>
    {
        public DateTime FechaInicio { get; }
        public DateTime FechaFin { get; }

        public ObtenerVentasPorFechasQuery(DateTime fechaInicio, DateTime fechaFin)
        {
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
        }
    }


}
