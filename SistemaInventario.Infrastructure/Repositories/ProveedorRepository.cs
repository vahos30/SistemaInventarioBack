using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Domain.Interfaces;
using SistemaInventario.Domain.Interfaces.SistemaInventario.Domain.Interfaces;
using SistemaInventario.Infrastructure.Persistence;

namespace SistemaInventario.Infrastructure.Repositories
{
    public class ProveedorRepository : IProveedorRepository
    {
        private readonly AppDbContext _context;
        public ProveedorRepository(AppDbContext context) => _context = context;

        public async Task AgregarAsync(Proveedor proveedor)
        {
            // Valida unicidad del NIT
            var existe = await _context.Proveedores.AnyAsync(p => p.NIT == proveedor.NIT);
            if (existe)
                throw new InvalidOperationException("Ya existe un proveedor con ese NIT.");

            await _context.Proveedores.AddAsync(proveedor);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Proveedor proveedor)
        {
            _context.Proveedores.Update(proveedor);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(Guid id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor != null)
            {
                _context.Proveedores.Remove(proveedor);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Proveedor?> ObtenerPorIdAsync(Guid id)
            => await _context.Proveedores.FindAsync(id);

        public async Task<IEnumerable<Proveedor>> ObtenerTodosAsync()
            => await _context.Proveedores.ToListAsync();

        public async Task<Proveedor?> ObtenerPorNitAsync(string nit)
            => await _context.Proveedores.FirstOrDefaultAsync(p => p.NIT == nit);

        public async Task EliminarPorNitAsync(string nit)
        {
            var proveedor = await ObtenerPorNitAsync(nit);
            if (proveedor != null)
            {
                _context.Proveedores.Remove(proveedor);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ActualizarPorNitAsync(string nit, Proveedor proveedorActualizado)
        {
            var proveedor = await ObtenerPorNitAsync(nit);
            if (proveedor != null)
            {
                proveedor.Nombre = proveedorActualizado.Nombre;
                proveedor.RazonSocial = proveedorActualizado.RazonSocial;
                proveedor.Telefono = proveedorActualizado.Telefono;
                proveedor.Email = proveedorActualizado.Email;
                proveedor.Activo = proveedorActualizado.Activo;
                await _context.SaveChangesAsync();
            }
        }
    }
}
