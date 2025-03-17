using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaInventario.Domain.Entities;

namespace SistemaInventario.Domain.Interfaces
{
    public interface IReciboRepository
    {
        // Metodo para obtener todos los recibos del dia actual
        Task<IEnumerable<Recibo>> ObtenerVentasDiariasAsync();

        // Metodo para obtener los recibos en un rango de fechas
        Task<IEnumerable<Recibo>> ObtenerVentasPorFechaAsync(DateTime fechaInicio, DateTime fechaFin);
    }
}
