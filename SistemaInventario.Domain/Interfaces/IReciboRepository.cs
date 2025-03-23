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
        //Agregar un nuevo recibo
        Task AgregarAsync(Recibo recibo);
        // Metodo para obtener todos los recibos del dia actual
        Task<IEnumerable<Recibo>> ObtenerVentasDiariasAsync();

        // Metodo para obtener los recibos en un rango de fechas
        Task<IEnumerable<Recibo>> ObtenerVentasPorFechaAsync(DateTime fechaInicio, DateTime fechaFin);

        // Metodo para obtener un recibo por su Id
        Task<Recibo?> ObtenerPorIdAsync(Guid id);

        // Metodo para obtener los recibos de un cliente
        Task<IEnumerable<Recibo>> ObtenerRecibosPorClientesAsync(Guid clienteId);
    }
}
