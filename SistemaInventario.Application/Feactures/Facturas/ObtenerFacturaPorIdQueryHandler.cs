using MediatR;
using SistemaInventario.Application.DTOs;
using SistemaInventario.Domain.Interfaces;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;

public class ObtenerFacturaPorIdQueryHandler : IRequestHandler<ObtenerFacturaPorIdQuery, FacturaDto>
{
    private readonly IFacturaRepository _facturaRepository;
    private readonly IMapper _mapper;

    public ObtenerFacturaPorIdQueryHandler(IFacturaRepository facturaRepository, IMapper mapper)
    {
        _facturaRepository = facturaRepository;
        _mapper = mapper;
    }

    public async Task<FacturaDto> Handle(ObtenerFacturaPorIdQuery request, CancellationToken cancellationToken)
    {
        var factura = await _facturaRepository.ObtenerPorIdAsync(request.Id);
        return factura == null ? null : _mapper.Map<FacturaDto>(factura);
    }
}