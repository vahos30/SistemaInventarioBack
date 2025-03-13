using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Infrastructure.Persistence;
using SistemaInventario.Domain.Interfaces;

namespace SistemaInventario.Infrastructure.Repositories
{
    // Implementacion del repositorio de productos usando Entity Framework core
    public class ProductoRepository : IProductoRepository
    {
        private readonly AppDbContext _context;
        // Inyeccion de dependencias a traves del constructor
        public ProductoRepository(AppDbContext context) => _context = context;

        public async Task AgregarAsync(Producto producto)
        {
            await _context.Productos.AddAsync(producto);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(Guid id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto is not null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ActualizarAsync(Producto producto)
        {
            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Producto>> ObtenerTodosAsync() =>
            await _context.Productos.ToListAsync();

        public async Task<Producto?> ObtenerPorIdsync(Guid id) =>
            await _context.Productos.FindAsync(id);
    }
}
