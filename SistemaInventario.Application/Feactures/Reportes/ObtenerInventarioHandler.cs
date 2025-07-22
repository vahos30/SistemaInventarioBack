using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SistemaInventario.Application.DTOs;
using SistemaInventario.Domain.Interfaces;

namespace SistemaInventario.Application.Feactures.Reportes
{
    public class ObtenerInventarioHandler : IRequestHandler<ObtenerInventarioQuery, IEnumerable<ProductoDto>>
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IMapper _mapper;

        public ObtenerInventarioHandler(IProductoRepository productoRepository, IMapper mapper)
        {
            _productoRepository = productoRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductoDto>> Handle(
            ObtenerInventarioQuery request,
            CancellationToken cancellationToken)
        {
            // Obtener todos los productos
            var productos = await _productoRepository.ObtenerTodosAsync();

            return _mapper.Map<IEnumerable<ProductoDto>>(productos);
        }
    }
}
