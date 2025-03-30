using MediatR;
using SistemaInventario.Domain.Interfaces;

namespace SistemaInventario.Application.Feactures.Productos
{
    public class ExisteProductoQuery : IRequest<bool>
    {
        public Guid ProductoId { get; set; }
        public ExisteProductoQuery(Guid productoId) => ProductoId = productoId;
    }

    public class ExisteProductoQueryHandler : IRequestHandler<ExisteProductoQuery, bool>
    {
        private readonly IProductoRepository _productoRepository;
        public ExisteProductoQueryHandler(IProductoRepository productoRepository)
            => _productoRepository = productoRepository;

        public async Task<bool> Handle(ExisteProductoQuery request, CancellationToken cancellationToken)
            => await _productoRepository.ExisteAsync(request.ProductoId);
    }
}
