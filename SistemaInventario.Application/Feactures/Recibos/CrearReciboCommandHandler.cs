using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MediatR;
using SistemaInventario.Application.DTOs;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Domain.Interfaces;
using SistemaInventario.Infrastructure.Persistence;

namespace SistemaInventario.Application.Feactures.Recibos
{
    public class CrearReciboCommandHandler : IRequestHandler<CrearReciboCommand, ReciboDto>
    {
        private readonly IReciboRepository _reciboRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CrearReciboCommandHandler(
            IReciboRepository reciboRepository,
            IProductoRepository productoRepository,
            AppDbContext context,
            IMapper mapper)
        {
            _reciboRepository = reciboRepository;
            _productoRepository = productoRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<ReciboDto> Handle(CrearReciboCommand request, CancellationToken cancellationToken)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Mapear y crear el recibo
                var recibo = new Recibo
                {
                    ClienteId = request.ClienteId,
                    Fecha = request.Fecha,
                    Detalles = _mapper.Map<List<DetalleRecibo>>(request.Detalles)
                };

                await _reciboRepository.AgregarAsync(recibo);

                // Actualizar el stock de cada producto
                foreach (var detalle in recibo.Detalles)
                {
                    var producto = await _productoRepository.ObtenerPorIdsync(detalle.ProductoId);
                    if (producto == null)
                        throw new Exception($"Producto con ID {detalle.ProductoId} no encontrado.");

                    if (producto.CantidadStock < detalle.Cantidad)
                        throw new Exception($"Stock insuficiente para el producto {producto.Nombre}.");

                    producto.CantidadStock -= detalle.Cantidad;
                    await _productoRepository.ActualizarAsync(producto);
                }

                await transaction.CommitAsync(); // Confirmar transacción
                return _mapper.Map<ReciboDto>(recibo);
            }
            catch
            {
                await transaction.RollbackAsync(); // Revertir en caso de error
                throw;
            }
        }
    }
}
