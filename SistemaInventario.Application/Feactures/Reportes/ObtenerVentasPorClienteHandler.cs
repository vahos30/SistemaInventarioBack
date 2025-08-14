using MediatR;
using SistemaInventario.Application.DTOs;
using SistemaInventario.Domain.Interfaces;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;

public class ObtenerVentasPorClienteHandler : IRequestHandler<ObtenerVentasPorClienteQuery, VentasPorClienteDto>
{
    private readonly IReciboRepository _reciboRepository;
    private readonly IFacturaRepository _facturaRepository;
    private readonly IMapper _mapper;

    public ObtenerVentasPorClienteHandler(IReciboRepository reciboRepository, IFacturaRepository facturaRepository, IMapper mapper)
    {
        _reciboRepository = reciboRepository;
        _facturaRepository = facturaRepository;
        _mapper = mapper;
    }

    public async Task<VentasPorClienteDto> Handle(ObtenerVentasPorClienteQuery request, CancellationToken cancellationToken)
    {
        var recibos = await _reciboRepository.ObtenerRecibosPorClientesAsync(request.ClienteId); 
        var facturas = await _facturaRepository.ObtenerFacturasPorClienteAsync(request.ClienteId);

        return new VentasPorClienteDto
        {
            Recibos = _mapper.Map<List<ReciboDto>>(recibos),
            Facturas = _mapper.Map<List<FacturaDto>>(facturas)
        };
    }
}