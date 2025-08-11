using MediatR;
using SistemaInventario.Application.DTOs;
using SistemaInventario.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

public class ObtenerFacturasAnuladasQuery : IRequest<List<FacturaAnuladaDto>> { }

public class ObtenerFacturasAnuladasQueryHandler : IRequestHandler<ObtenerFacturasAnuladasQuery, List<FacturaAnuladaDto>>
{
    private readonly AppDbContext _context;

    public ObtenerFacturasAnuladasQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<FacturaAnuladaDto>> Handle(ObtenerFacturasAnuladasQuery request, CancellationToken cancellationToken)
    {
        return await _context.Facturas
            .Where(f => f.Anulada)
            .Include(f => f.NotaCredito)
            .Select(f => new FacturaAnuladaDto
            {
                NumeroFactura = f.NumeroFactura,
                NumeroNotaCredito = f.NotaCredito != null ? f.NotaCredito.NumeroNotaCredito : null,
                MotivoAnulacion = f.MotivoAnulacion,
                FechaAnulacion = f.FechaAnulacion,
                Total = f.NotaCredito != null ? f.NotaCredito.Total : 0
            })
            .ToListAsync(cancellationToken);
    }
}