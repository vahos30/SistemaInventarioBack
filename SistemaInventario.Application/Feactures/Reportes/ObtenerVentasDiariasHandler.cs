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
            // Obtener la fecha actual en la zona de Colombia
            var colombiaZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            var fechaColombia = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, colombiaZone);

            // Calcular el rango de hoy en Colombia
            var fechaInicio = fechaColombia.Date;
            var fechaFin = fechaInicio.AddDays(1);

            var recibosResult = await _reciboRepository.ObtenerVentasPorFechaAsync(fechaInicio, fechaFin);
            var facturas = await _facturaRepository.ObtenerFacturasPorFechaAsync(fechaInicio, fechaFin);

            return new VentasDiariasDto
            {
                Recibos = _mapper.Map<List<ReciboDto>>(recibosResult),
                Facturas = _mapper.Map<List<FacturaDto>>(facturas)
            };
        }
    }
}

