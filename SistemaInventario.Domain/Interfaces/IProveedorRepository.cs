using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaInventario.Domain.Entities;

namespace SistemaInventario.Domain.Interfaces
{
    namespace SistemaInventario.Domain.Interfaces
    {
        public interface IProveedorRepository
        {
            Task AgregarAsync(Proveedor proveedor);
            Task ActualizarAsync(Proveedor proveedor);
            Task EliminarAsync(Guid id);
            Task<Proveedor?> ObtenerPorIdAsync(Guid id);
            Task<IEnumerable<Proveedor>> ObtenerTodosAsync();

            // Métodos por NIT
            Task<Proveedor?> ObtenerPorNitAsync(string nit);
            Task EliminarPorNitAsync(string nit);
            Task ActualizarPorNitAsync(string nit, Proveedor proveedorActualizado);
        }
    }
}
