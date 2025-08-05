using Microsoft.EntityFrameworkCore;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Domain.Interfaces;
using SistemaInventario.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class NotaCreditoRepository : INotaCreditoRepository
{
    private readonly AppDbContext _context;

    public NotaCreditoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AgregarAsync(NotaCredito notaCredito)
    {
        await _context.NotasCredito.AddAsync(notaCredito);
        await _context.SaveChangesAsync();
    }

    public async Task<NotaCredito?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.NotasCredito
            .Include(nc => nc.Detalles)
            .FirstOrDefaultAsync(nc => nc.Id == id);
    }

    public async Task<IEnumerable<NotaCredito>> ObtenerNotasCreditoAsync()
    {
        return await _context.NotasCredito
            .Include(nc => nc.Detalles)
            .ToListAsync();
    }
}