
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

        public async Task AgregarAsync(Recibo recibo)
        {
            recibo.Fecha = DateTime.SpecifyKind(recibo.Fecha, DateTimeKind.Utc);
            await _context.Recibos.AddAsync(recibo);
            await _context.SaveChangesAsync();
        }

        public async Task<(IEnumerable<Recibo> Recibos, decimal Total)> ObtenerVentasDiariasAsync(DateTime? fechaReferencia)
        {
            var hoy = DateTime.SpecifyKind(fechaReferencia?.ToUniversalTime().Date ?? DateTime.UtcNow.Date, DateTimeKind.Utc);

            var fin = hoy.AddDays(1);

            var query = _context.Recibos
                .Include(r => r.Cliente)
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.Producto)
                .Where(r => r.Fecha >= hoy && r.Fecha < fin)
                .AsNoTracking();

            var recibosDelDia = await query.ToListAsync();
            var total = recibosDelDia.Sum(r => r.Total);

            return (recibosDelDia, total);
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

        public async Task<IEnumerable<Recibo>> ObtenerRecibosAsync()
        {
            return await _context.Recibos
                .Include(r => r.Cliente)
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.Producto)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
