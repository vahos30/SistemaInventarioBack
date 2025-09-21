using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SistemaInventario.Application.DTOs;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Domain.Interfaces;

namespace SistemaInventario.Application.Feactures.Clientes
{
    /// <summary>
    /// Handler que procesa el comando para crear un nuevo cliente.
    /// </summary>
    public class CrearClienteCommandHandler : IRequestHandler<CrearClienteCommand, ClienteDto>
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IMapper _mapper;
        private readonly CiudadService _ciudadService;

        public CrearClienteCommandHandler(IClienteRepository clienteRepository, IMapper mapper, CiudadService ciudadService)
        {
            _clienteRepository = clienteRepository;
            _mapper = mapper;
            _ciudadService = ciudadService;
        }

        public async Task<ClienteDto> Handle(CrearClienteCommand request, CancellationToken cancellationToken)
        {
            // Consultar la ciudad por código
            var ciudades = await _ciudadService.ObtenerCiudadesAsync();
            var ciudad = ciudades.FirstOrDefault(c => c.id == request.CiudadId);

            if (ciudad == null)
                throw new Exception("Ciudad no encontrada");

            var cliente = new Cliente
            {
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                TipoDocumento = request.TipoDocumento,
                NumeroDocumento = request.NumeroDocumento,
                Telefono = request.Telefono,
                Direccion = request.Direccion,
                Email = request.Email,
                CodigoCiudad = ciudad.code,
                Ciudad = ciudad.name,
                Departamento = ciudad.department,
                CiudadId = ciudad.id,
                IdTipoOrganizacion = request.IdTipoOrganizacion,
                IdTributo = request.IdTributo,
                IdTipoDocumentoIdentidad = request.IdTipoDocumentoIdentidad,
                RazonSocial = request.RazonSocial,
                    
                
            };

            await _clienteRepository.AgregarAsync(cliente);

            // Mapear la entidad a DTO para devolver solo los datos necesarios
            return _mapper.Map<ClienteDto>(cliente);
        }
    }
}
