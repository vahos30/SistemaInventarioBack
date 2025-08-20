using MediatR;
using SistemaInventario.Domain.Interfaces;
using SistemaInventario.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

public class AnularFacturaCommandHandler : IRequestHandler<AnularFacturaCommand>
{
    private readonly IFacturaRepository _facturaRepository;
    private readonly INotaCreditoRepository _notaCreditoRepository;
    private readonly IProductoRepository _productoRepository;

    public AnularFacturaCommandHandler(
        IFacturaRepository facturaRepository,
        INotaCreditoRepository notaCreditoRepository,
        IProductoRepository productoRepository
    )
    {
        _facturaRepository = facturaRepository;
        _notaCreditoRepository = notaCreditoRepository;
        _productoRepository = productoRepository;
    }

    public async Task<Unit> Handle(AnularFacturaCommand request, CancellationToken cancellationToken)
    {
        var factura = await _facturaRepository.ObtenerPorIdAsync(request.FacturaId);
        if (factura == null || factura.Anulada)
            return Unit.Value;

        factura.Anulada = true;
        factura.MotivoAnulacion = request.Motivo;
        factura.FechaAnulacion = DateTime.UtcNow;

        // Devolver stock al inventario
        foreach (var detalle in factura.Detalles)
        {
            var producto = await _productoRepository.ObtenerPorIdsync(detalle.ProductoId);
            if (producto != null)
            {
                int stockAnterior = producto.CantidadStock;
                producto.CantidadStock += detalle.Cantidad;

                // Si el stock era 0 y ahora es mayor que 0, activar el producto
                if (stockAnterior == 0 && producto.CantidadStock > 0)
                {
                    producto.Activo = true;
                }

                await _productoRepository.ActualizarAsync(producto);
            }
        }

        // Generar nota crédito
        var notaCredito = new NotaCredito
        {
            NumeroNotaCredito = $"NC-{factura.NumeroFactura}",
            FacturaId = factura.Id,
            Fecha = DateTime.UtcNow,
            Motivo = request.Motivo,
            Total = factura.Total,
            Detalles = factura.Detalles.Select(d => new DetalleNotaCredito
            {
                ProductoId = d.ProductoId,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario
            }).ToList()
        };

        await _notaCreditoRepository.AgregarAsync(notaCredito);

        factura.NotaCreditoId = notaCredito.Id;
        await _facturaRepository.ActualizarAsync(factura);

        return Unit.Value;
    }
}