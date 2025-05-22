using MediatR;
using SistemaInventario.Application.DTOs;
using SistemaInventario.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaInventario.Application.Feactures.Reportes
{
    public class ObtenerComprasPorFechasQueryHandler : IRequestHandler<ObtenerComprasPorFechasQuery, List<CompraReporteDto>>
    {
        private readonly ICompraRepository _compraRepository;

        public ObtenerComprasPorFechasQueryHandler(ICompraRepository compraRepository)
        {
            _compraRepository = compraRepository;
        }

        public async Task<List<CompraReporteDto>> Handle(ObtenerComprasPorFechasQuery request, CancellationToken cancellationToken)
        {
            var compras = await _compraRepository.ObtenerConDetallesYProveedorAsync(); // Este método debe traer compras con detalles y proveedor

            // Filtrar por fechas si aplica
            if (request.FechaInicio.HasValue)
                compras = compras.Where(c => c.Fecha >= request.FechaInicio.Value).ToList();
            if (request.FechaFin.HasValue)
                compras = compras.Where(c => c.Fecha <= request.FechaFin.Value).ToList();

            var reporte = compras.Select(c => new CompraReporteDto
            {
                CompraId = c.Id,
                Fecha = c.Fecha,
                ProveedorNombre = c.Proveedor?.Nombre ?? "N/A",
                Total = c.Total,
                Detalles = c.Detalles.Select(d => new CompraDetalleReporteDto
                {
                    ProductoNombre = d.Producto?.Nombre ?? "N/A",
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Total = d.Cantidad * d.PrecioUnitario
                }).ToList()
            }).ToList();

            return reporte;
        }
    }
}