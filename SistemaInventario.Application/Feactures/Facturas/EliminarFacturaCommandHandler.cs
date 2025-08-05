using MediatR;
using SistemaInventario.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

public class EliminarFacturaCommandHandler : IRequestHandler<EliminarFacturaCommand>
{
    private readonly IFacturaRepository _facturaRepository;

    public EliminarFacturaCommandHandler(IFacturaRepository facturaRepository)
    {
        _facturaRepository = facturaRepository;
    }

    public async Task<Unit> Handle(EliminarFacturaCommand request, CancellationToken cancellationToken)
    {
        await _facturaRepository.EliminarAsync(request.Id);
        return Unit.Value;
    }
}