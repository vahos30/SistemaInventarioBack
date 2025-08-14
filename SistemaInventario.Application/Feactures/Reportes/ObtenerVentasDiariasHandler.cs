using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SistemaInventario.Application.DTOs;
using SistemaInventario.Domain.Interfaces;

namespace SistemaInventario.Application.Feactures.Reportes
{
    public class ObtenerVentasDiariasHandler : IRequestHandler<ObtenerVentasDiariasQuery, VentasDiariasDto>
    {
        private readonly IReciboRepository _reciboRepository;
        private readonly IFacturaRepository _facturaRepository;
        private readonly IMapper _mapper;

        public ObtenerVentasDiariasHandler(IReciboRepository reciboRepository, IFacturaRepository facturaRepository, IMapper mapper)
        {
            _reciboRepository = reciboRepository;
            _facturaRepository = facturaRepository;
            _mapper = mapper;
        }

        public async Task<VentasDiariasDto> Handle(ObtenerVentasDiariasQuery request, CancellationToken cancellationToken)
        {
            var recibosResult = await _reciboRepository.ObtenerVentasDiariasAsync(null);
            var facturas = await _facturaRepository.ObtenerFacturasDiariasAsync(null);

            return new VentasDiariasDto
            {
                Recibos = _mapper.Map<List<ReciboDto>>(recibosResult.Recibos),
                Facturas = _mapper.Map<List<FacturaDto>>(facturas)
            };
        }
    }
}

