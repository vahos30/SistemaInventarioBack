using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class CrearClienteCommandHandler : IRequestHandler<CrearClienteCommand, ClienteDto>
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IMapper _mapper;

        public CrearClienteCommandHandler(IClienteRepository clienteRepository, IMapper mapper)
        {
            _clienteRepository = clienteRepository;
            _mapper = mapper;
        }

        public async Task<ClienteDto> Handle(CrearClienteCommand request, CancellationToken cancellationToken)
        {
            var cliente = new Cliente
            {
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                NumeroDocumento = request.NumeroDocumento,
                Telefono = request.Telefono,
                Direccion = request.Direccion,
                Email = request.Email
            };

            await _clienteRepository.AgregarAsync(cliente);

            // Mapear la entidad a DTO para devolver solo los datos necesarios
            return _mapper.Map<ClienteDto>(cliente);
        }
    }
}
