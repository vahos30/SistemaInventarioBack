using Microsoft.EntityFrameworkCore;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Domain.Interfaces;
using SistemaInventario.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaInventario.Infrastructure.Repositories
{
    public class FacturaRepository : IFacturaRepository
    {
        private readonly AppDbContext _context;

        public FacturaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AgregarAsync(Factura factura)
        {
            await _context.Facturas.AddAsync(factura);
            await _context.SaveChangesAsync();
        }

        public async Task<Factura?> ObtenerPorIdAsync(Guid id)
        {
            return await _context.Facturas
                .Include(f => f.Cliente)
                .Include(f => f.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<IEnumerable<Factura>> ObtenerFacturasAsync()
        {
            return await _context.Facturas
                .Include(f => f.Cliente)
                .Include(f => f.Detalles)
                    .ThenInclude(d => d.Producto)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task EliminarAsync(Guid id)
        {
            var factura = await _context.Facturas
                .Include(f => f.Detalles)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (factura != null)
            {
                _context.Facturas.Remove(factura);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ActualizarAsync(Factura factura)
        {
            _context.Facturas.Update(factura);
            await _context.SaveChangesAsync();
        }
    }
}