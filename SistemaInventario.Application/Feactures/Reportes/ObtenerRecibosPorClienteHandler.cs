using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SistemaInventario.Application.DTOs;
using SistemaInventario.Application.Feactures.Recibos;
using SistemaInventario.Domain.Interfaces;

namespace SistemaInventario.Application.Feactures.Reportes
{
    public class ObtenerRecibosPorClienteHandler : IRequestHandler<ObtenerRecibosPorClienteQuery, IEnumerable<ReciboDto>>
    {
        private readonly IReciboRepository _reciboRepository;
        private readonly IMapper _mapper;

        public ObtenerRecibosPorClienteHandler(IReciboRepository reciboRepository, IMapper mapper)
        {
            _reciboRepository = reciboRepository;
            _mapper = mapper;
        }


        public async Task<IEnumerable<ReciboDto>> Handle(ObtenerRecibosPorClienteQuery request, CancellationToken cancellationToken)
        {
            var recibos = await _reciboRepository.ObtenerRecibosPorClientesAsync(request.ClienteId);
            return _mapper.Map<IEnumerable<ReciboDto>>(recibos);
        }
    }
}