using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SistemaInventario.Application.DTOs;
using SistemaInventario.Domain.Interfaces;

namespace SistemaInventario.Application.Feactures.Recibos
{
    public class ObtenerRecibosQueryHandler : IRequestHandler<ObtenerRecibosQuery, List<ReciboDto>>
    {
        private readonly IReciboRepository _reciboRepository;
        private readonly IMapper _mapper;

        public ObtenerRecibosQueryHandler(IReciboRepository reciboRepository, IMapper mapper)
        {
            _reciboRepository = reciboRepository;
            _mapper = mapper;
        }

        public async Task<List<ReciboDto>> Handle(ObtenerRecibosQuery request, CancellationToken cancellationToken)
        {
            var recibos = await _reciboRepository.ObtenerRecibosAsync();
            return _mapper.Map<List<ReciboDto>>(recibos);
        }
    }
}

