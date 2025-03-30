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
    public class ObtenerVentasDiariasHandler : IRequestHandler<ObtenerVentasDiariasQuery, IEnumerable<ReciboDto>>
    {
        private readonly IReciboRepository _reciboRepository;
        private readonly IMapper _mapper;

        public ObtenerVentasDiariasHandler(IReciboRepository reciboRepository, IMapper mapper)
        {
            _reciboRepository = reciboRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReciboDto>> Handle(ObtenerVentasDiariasQuery request, CancellationToken cancellationToken)
        {
            // Pasamos null para fechaReferencia (o request.FechaReferencia si el query lo tiene)
            var result = await _reciboRepository.ObtenerVentasDiariasAsync(null);
            return _mapper.Map<IEnumerable<ReciboDto>>(result.Recibos);
        }
    }
}

