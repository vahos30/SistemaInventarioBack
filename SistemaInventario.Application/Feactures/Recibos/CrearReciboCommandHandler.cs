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

namespace SistemaInventario.Application.Feactures.Recibos
{
    /// <summary>
    /// Handler que procesa el comando para crear un recibo.
    /// </summary>
    public class CrearReciboCommandHandler : IRequestHandler<CrearReciboCommand, ReciboDto>
    {
        private readonly IReciboRepository _reciboRepository;
        private readonly IMapper _mapper;

        public CrearReciboCommandHandler(IReciboRepository reciboRepository, IMapper mapper)
        {
            _reciboRepository = reciboRepository;
            _mapper = mapper;
        }

        public async Task<ReciboDto> Handle(CrearReciboCommand request, CancellationToken cancellationToken)
        {
            var recibo = new Recibo
            {
                ClienteId = request.ClienteId,
                Fecha = request.Fecha,

                Detalles = _mapper.Map<List<DetalleRecibo>>(request.Detalles)
            };

            await _reciboRepository.AgregarAsync(recibo);

            return _mapper.Map<ReciboDto>(recibo);
        }
    }
}
