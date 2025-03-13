using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaInventario.Domain.Entities;

namespace SistemaInventario.Domain.Interfaces
{
    // Contrato para operaciones CRUD sobre la entidad Producto
    public interface IProductoRepository
    {
        // Obtener todos los productos
        Task<IEnumerable<Producto>> ObtenerTodosAsync();
        // Obtener un producto por su Id
        Task<Producto?> ObtenerPorIdsync(Guid id);
        // Crear un nuevo producto
        Task AgregarAsync(Producto producto);
        // Actualizar un producto existente
        Task ActualizarAsync(Producto producto);
        // Eliminar un producto existente
        Task EliminarAsync(Guid id);
    }
}
