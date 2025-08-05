using MediatR;
using SistemaInventario.Application.DTOs;
using SistemaInventario.Domain.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class ObtenerFacturasQueryHandler : IRequestHandler<ObtenerFacturasQuery, List<FacturaDto>>
{
    private readonly IFacturaRepository _facturaRepository;
    private readonly IMapper _mapper;

    public ObtenerFacturasQueryHandler(IFacturaRepository facturaRepository, IMapper mapper)
    {
        _facturaRepository = facturaRepository;
        _mapper = mapper;
    }

    public async Task<List<FacturaDto>> Handle(ObtenerFacturasQuery request, CancellationToken cancellationToken)
    {
        var facturas = await _facturaRepository.ObtenerFacturasAsync();
        return _mapper.Map<List<FacturaDto>>(facturas);
    }
}