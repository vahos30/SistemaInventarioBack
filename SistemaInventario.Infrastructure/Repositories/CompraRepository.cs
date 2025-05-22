using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Domain.Interfaces;
using SistemaInventario.Infrastructure.Persistence;

namespace SistemaInventario.Infrastructure.Repositories
{
    public class CompraRepository : ICompraRepository
    {
        private readonly AppDbContext _context;

        public CompraRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AgregarAsync(Compra compra)
        {
            await _context.Compras.AddAsync(compra);
            await _context.SaveChangesAsync();
        }

        public async Task<Compra?> ObtenerPorIdAsync(Guid id)
        {
            return await _context.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Compra>> ObtenerTodasAsync()
        {
            return await _context.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.Detalles)
                    .ThenInclude(d => d.Producto)
                .ToListAsync();
        }

        public async Task ActualizarAsync(Compra compra) // <--- Nuevo método
        {
            _context.Compras.Update(compra);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(Guid id) // <--- Nuevo método
        {
            var compra = await _context.Compras.FindAsync(id);
            if (compra != null)
            {
                _context.Compras.Remove(compra);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Compra>> ObtenerConDetallesYProveedorAsync()
        {
            return await _context.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.Detalles)
                    .ThenInclude(d => d.Producto)
                .ToListAsync();
        }
    }
}
