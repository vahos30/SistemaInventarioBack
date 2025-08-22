using MediatR;
using AutoMapper;
using SistemaInventario.Domain.Interfaces;
using SistemaInventario.Application.DTOs;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

public class CrearFacturaCommandHandler : IRequestHandler<CrearFacturaCommand, FacturaDto>
{
    private readonly IFacturaRepository _facturaRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CrearFacturaCommandHandler(
        IFacturaRepository facturaRepository,
        IProductoRepository productoRepository,
        AppDbContext context,
        IMapper mapper)
    {
        _facturaRepository = facturaRepository;
        _productoRepository = productoRepository;
        _context = context;
        _mapper = mapper;
    }

    public async Task<FacturaDto> Handle(CrearFacturaCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Buscar el último número de factura
            var ultimaFactura = await _context.Facturas
                .OrderByDescending(f => f.NumeroFactura)
                .FirstOrDefaultAsync();

            int siguienteNumero = 501; // Valor inicial
            if (ultimaFactura != null && int.TryParse(ultimaFactura.NumeroFactura.Replace("F", ""), out int ultimo))
            {
                siguienteNumero = ultimo + 1;
            }

            if (siguienteNumero > 1500)
                throw new Exception("Se ha alcanzado el límite de numeración de facturas.");

            var colombiaZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            var fechaColombia = TimeZoneInfo.ConvertTime(request.Fecha, colombiaZone);

            var factura = new Factura
            {
                NumeroFactura = $"F{siguienteNumero}",
                ClienteId = request.ClienteId,
                Fecha = fechaColombia,
                FormaPago = request.FormaPago,
                Detalles = _mapper.Map<List<DetalleFactura>>(request.Detalles)
            };

            await _facturaRepository.AgregarAsync(factura);

            foreach (var detalle in factura.Detalles)
            {
                var producto = await _productoRepository.ObtenerPorIdsync(detalle.ProductoId);
                if (producto == null)
                    throw new Exception($"Producto con ID {detalle.ProductoId} no encontrado.");

                if (producto.CantidadStock < detalle.Cantidad)
                    throw new Exception($"Stock insuficiente para el producto {producto.Nombre}.");

                producto.CantidadStock -= detalle.Cantidad;
                await _productoRepository.ActualizarAsync(producto);
            }

            await transaction.CommitAsync();
            return _mapper.Map<FacturaDto>(factura);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}