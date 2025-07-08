using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SistemaInventario.Domain.Entities;

namespace SistemaInventario.Domain.Interfaces
{
    public interface IReciboRepository
    {
        // Agregar un nuevo recibo
        Task AgregarAsync(Recibo recibo);

        // Método para obtener todos los recibos del día actual,
        // devolviendo un tuple con la lista de recibos y el total calculado.
        Task<(IEnumerable<Recibo> Recibos, decimal Total)> ObtenerVentasDiariasAsync(DateTime? fechaReferencia);

        // Método para obtener los recibos en un rango de fechas
        Task<IEnumerable<Recibo>> ObtenerVentasPorFechaAsync(DateTime fechaInicio, DateTime fechaFin);

        // Método para obtener un recibo por su Id
        Task<Recibo?> ObtenerPorIdAsync(Guid id);

        // Método para obtener los recibos de un cliente
        Task<IEnumerable<Recibo>> ObtenerRecibosPorClientesAsync(Guid clienteId);

        // Método para obtener todos los recibos
        Task<IEnumerable<Recibo>> ObtenerRecibosAsync();

        //Metodo para eliminar un recibo por su Id
        Task EliminarAsync(Guid id);
    }
}

