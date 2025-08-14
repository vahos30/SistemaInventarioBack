using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SistemaInventario.Application.DTOs;
using SistemaInventario.Domain.Interfaces;

namespace SistemaInventario.Application.Feactures.Reportes
{
    public class ObtenerVentasPorFechasHandler : IRequestHandler<ObtenerVentasPorFechasQuery, VentasPorFechasDto>
    {
        private readonly IReciboRepository _reciboRepository;
        private readonly IFacturaRepository _facturaRepository;
        private readonly IMapper _mapper;

        public ObtenerVentasPorFechasHandler(IReciboRepository reciboRepository, IFacturaRepository facturaRepository, IMapper mapper)
        {
            _reciboRepository = reciboRepository;
            _facturaRepository = facturaRepository;
            _mapper = mapper;
        }

        public async Task<VentasPorFechasDto> Handle(ObtenerVentasPorFechasQuery request, CancellationToken cancellationToken)
        {
            if (request.FechaInicio > request.FechaFin)
            {
                throw new ArgumentException("La fecha de inicio no puede ser mayor a la fecha fin");
            }

            var recibos = await _reciboRepository.ObtenerVentasPorFechaAsync(request.FechaInicio, request.FechaFin);
            var facturas = await _facturaRepository.ObtenerFacturasPorFechaAsync(request.FechaInicio, request.FechaFin);

            return new VentasPorFechasDto
            {
                Recibos = _mapper.Map<List<ReciboDto>>(recibos),
                Facturas = _mapper.Map<List<FacturaDto>>(facturas)
            };
        }
    }
}
