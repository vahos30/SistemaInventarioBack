using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SistemaInventario.Domain.Entities;

public interface IFacturaRepository
{
    Task AgregarAsync(Factura factura);
    Task<Factura?> ObtenerPorIdAsync(Guid id);
    Task<IEnumerable<Factura>> ObtenerFacturasAsync();
    Task EliminarAsync(Guid id);
    Task ActualizarAsync(Factura factura);
}