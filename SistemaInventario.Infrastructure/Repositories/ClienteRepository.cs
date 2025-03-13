using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Domain.Interfaces;
using SistemaInventario.Infrastructure.Persistence;

namespace SistemaInventario.Infrastructure.Repositories
{
    // Implementacion del repositorio de clientes usando Entity Framework core
    public class ClienteRepository : IClienteRepository
    {
        private readonly AppDbContext _context;

        // Inyeccion de dependencias a traves del constructor
        public ClienteRepository(AppDbContext context) => _context = context;

        public async Task AgregarAsync(Cliente cliente)
        {
            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(Guid id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente is not null)
            {
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ActualizarAsync(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Cliente>> ObtenerTodosAsync() =>
            await _context.Clientes.ToListAsync();

        public async Task<Cliente?> ObtenerPorIdsync(Guid id) =>
            await _context.Clientes.FindAsync(id);

    }
}
