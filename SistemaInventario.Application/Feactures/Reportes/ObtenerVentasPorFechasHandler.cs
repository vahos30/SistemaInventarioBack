using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SistemaInventario.Application.DTOs;
using SistemaInventario.Domain.Interfaces;

namespace SistemaInventario.Application.Feactures.Reportes
{
    public class ObtenerVentasPorFechasHandler : IRequestHandler<ObtenerVentasPorFechasQuery, IEnumerable<ReciboDto>>
    {
        private readonly IReciboRepository _reciboRepository;
        private readonly IMapper _mapper;
        public ObtenerVentasPorFechasHandler(IReciboRepository reciboRepository, IMapper mapper)
        {
            _reciboRepository = reciboRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<ReciboDto>> Handle(ObtenerVentasPorFechasQuery request, CancellationToken cancellationToken)
        {
            var ventas = await _reciboRepository.ObtenerVentasPorFechaAsync(request.FechaInicio, request.FechaFin);
            return _mapper.Map<IEnumerable<ReciboDto>>(ventas);
        }
    }
}
