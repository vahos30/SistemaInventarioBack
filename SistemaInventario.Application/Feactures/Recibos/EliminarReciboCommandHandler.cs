using MediatR;
using SistemaInventario.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

public class EliminarReciboCommandHandler : IRequestHandler<EliminarReciboCommand>
{
    private readonly IReciboRepository _reciboRepository;
    private readonly IProductoRepository _productoRepository;

    public EliminarReciboCommandHandler(IReciboRepository reciboRepository, IProductoRepository productoRepository)
    {
        _reciboRepository = reciboRepository;
        _productoRepository = productoRepository;
    }

    public async Task<Unit> Handle(EliminarReciboCommand request, CancellationToken cancellationToken)
    {
        // 1. Obtener el recibo con sus detalles
        var recibo = await _reciboRepository.ObtenerPorIdAsync(request.Id);
        if (recibo == null)
            return Unit.Value;

        // 2. Por cada detalle, devolver el stock al producto
        foreach (var detalle in recibo.Detalles)
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

        // 3. Eliminar el recibo
        await _reciboRepository.EliminarAsync(request.Id);

        return Unit.Value;
    }
}