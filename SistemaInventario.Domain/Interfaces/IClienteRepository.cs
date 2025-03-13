using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaInventario.Domain.Entities;

namespace SistemaInventario.Domain.Interfaces
{
    // Contrato para operaciones CRUD sobre la entidad Cliente
    public interface IClienteRepository
    {
        // Obtener todos los clientes
        Task<IEnumerable<Cliente>> ObtenerTodosAsync();
        // Obtener un cliente por su Id
        Task<Cliente?> ObtenerPorIdsync(Guid id);
        // Crear un nuevo cliente
        Task AgregarAsync(Cliente cliente);
        // Actualizar un cliente existente
        Task ActualizarAsync(Cliente cliente);
        // Eliminar un cliente existente
        Task EliminarAsync (Guid id);
    }
}
