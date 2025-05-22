using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SistemaInventario.Domain.Entities;

namespace SistemaInventario.Domain.Interfaces
{
    public interface ICompraRepository
    {
        Task AgregarAsync(Compra compra);
        Task<Compra?> ObtenerPorIdAsync(Guid id);
        Task<IEnumerable<Compra>> ObtenerTodasAsync();
        Task ActualizarAsync(Compra compra);   
        Task EliminarAsync(Guid id);
        Task<List<Compra>> ObtenerConDetallesYProveedorAsync();
    }
}
