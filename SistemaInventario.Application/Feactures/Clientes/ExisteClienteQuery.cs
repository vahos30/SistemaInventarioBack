using MediatR;
using SistemaInventario.Domain.Interfaces;

namespace SistemaInventario.Application.Feactures.Clientes
{
    public class ExisteClienteQuery : IRequest<bool>
    {
        public Guid ClienteId { get; set; }
        public ExisteClienteQuery(Guid clienteId) => ClienteId = clienteId;
    }

    public class ExisteClienteQueryHandler : IRequestHandler<ExisteClienteQuery, bool>
    {
        private readonly IClienteRepository _clienteRepository;
        public ExisteClienteQueryHandler(IClienteRepository clienteRepository)
            => _clienteRepository = clienteRepository;

        public async Task<bool> Handle(ExisteClienteQuery request, CancellationToken cancellationToken)
            => await _clienteRepository.ExisteAsync(request.ClienteId);
    }
}