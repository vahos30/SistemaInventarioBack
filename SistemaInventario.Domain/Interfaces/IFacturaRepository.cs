using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SistemaInventario.Domain.Entities;

public interface IFacturaRepository
{
    Task AgregarAsync(Factura factura);
    Task<Factura?> ObtenerPorIdAsync(Guid id);
    Task<IEnumerable<Factura>> ObtenerFacturasAsync();
    Task<IEnumerable<Factura>> ObtenerFacturasPorClienteAsync(Guid clienteId);
    Task<IEnumerable<Factura>> ObtenerFacturasDiariasAsync(DateTime? fechaReferencia);
    Task<IEnumerable<Factura>> ObtenerFacturasPorFechaAsync(DateTime fechaInicio, DateTime fechaFin);
    Task EliminarAsync(Guid id);
    Task ActualizarAsync(Factura factura);
}