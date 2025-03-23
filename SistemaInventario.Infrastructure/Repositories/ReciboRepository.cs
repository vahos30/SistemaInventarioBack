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
    public class ReciboRepository : IReciboRepository
    {
        private readonly AppDbContext _context;

        public ReciboRepository(AppDbContext context) 
        { 
            _context = context; 
        
        }

        public async Task<IEnumerable<Recibo>> ObtenerVentasDiariasAsync() 
        {
            var hoy = DateTime.UtcNow.Date;
            return await _context.Recibos
                .Include(r => r.Cliente)          
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.Producto) 
                .Where(r => r.Fecha == hoy)
                .ToListAsync();

        }

        public async Task AgregarAsync(Recibo recibo)
        {
            await _context.Recibos.AddAsync(recibo);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Recibo>> ObtenerVentasPorFechaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.Recibos
                .Include(r => r.Detalles)
                .Where(r => r.Fecha >= fechaInicio && r.Fecha <= fechaFin)
                .ToListAsync();
        }

        public async Task<IEnumerable<Recibo>> ObtenerRecibosPorClientesAsync(Guid clienteId)
        {
            return await _context.Recibos
                .Include(r => r.Detalles)
                .Where(r => r.ClienteId == clienteId)
                .ToListAsync();
        }

        public async Task<Recibo?> ObtenerPorIdAsync(Guid id) =>
    await _context.Recibos
        .Include(r => r.Cliente)          
        .Include(r => r.Detalles)
            .ThenInclude(d => d.Producto) 
        .FirstOrDefaultAsync(r => r.Id == id);




    }
}
