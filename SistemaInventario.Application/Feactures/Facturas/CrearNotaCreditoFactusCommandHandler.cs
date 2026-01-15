using MediatR;
using SistemaInventario.Domain.Interfaces;
using SistemaInventario.Application.Services;
using SistemaInventario.Application.Mappers;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

public class CrearNotaCreditoFactusCommandHandler : IRequestHandler<CrearNotaCreditoFactusCommand, string>
{
    private readonly IFacturaRepository _facturaRepository;
    private readonly FactusFacturaService _factusFacturaService;
    private readonly INotaCreditoRepository _notaCreditoRepository;
    private readonly IProductoRepository _productoRepository; // <-- Agrega esta dependencia

    public CrearNotaCreditoFactusCommandHandler(
        IFacturaRepository facturaRepository,
        FactusFacturaService factusFacturaService,
        INotaCreditoRepository notaCreditoRepository,
        IProductoRepository productoRepository // <-- Inyéctala aquí
    )
    {
        _facturaRepository = facturaRepository;
        _factusFacturaService = factusFacturaService;
        _notaCreditoRepository = notaCreditoRepository;
        _productoRepository = productoRepository; // <-- Asigna aquí
    }

    public async Task<string> Handle(CrearNotaCreditoFactusCommand request, CancellationToken cancellationToken)
    {
        // 1. Obtener la factura original
        var factura = await _facturaRepository.ObtenerPorIdAsync(request.FacturaId);
        if (factura == null)
            throw new Exception("Factura no encontrada.");

        // Generar la referencia antes de usarla
        var referencia = $"NC-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid().ToString().Substring(0, 8)}";

        // 2. Mapear los datos de la factura a la estructura de la nota crédito
        var notaCreditoRequest = new
        {
            numbering_range_id = 735,
            correction_concept_code = request.CorrectionConceptCode,
            customization_id = request.CustomizationId,
            bill_id = factura.FactusBillId,
            reference_code = referencia,
            payment_method_code = request.PaymentMethodCode,
            send_email = true,
            observation = request.Observation,
            establishment = new
            {
                name = "TECNOFRIO DISTRIBUCIONES S.A.S.",
                address = "CARRERA 99 65 265",
                phone_number = "(57) 3113740874",
                email = "administracion@tecnofriodistribuciones.com.co",
                municipality_id = "80"
            },
            customer = FactusMapper.MapClienteToFactusCustomer(factura.Cliente),
            items = FactusMapper.MapDetallesToFactusItems(factura.Detalles)
        };

        // 3. Llamar al servicio externo
        var result = await _factusFacturaService.CrearNotaCreditoAsync(notaCreditoRequest);

        // 4. Deserializar la respuesta
        var factusResponse = JsonSerializer.Deserialize<FactusNotaCreditoResponse>(result);

        // 5. Crear la entidad NotaCredito
        var notaCredito = new NotaCredito
        {
            NumeroNotaCredito = factusResponse.data.credit_note.number, // o .reference_code si prefieres
            FacturaId = request.FacturaId,
            Fecha = DateTime.ParseExact(factusResponse.data.credit_note.validated, "dd-MM-yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture),
            Motivo = factusResponse.data.credit_note.observation,
            Total = decimal.Parse(factusResponse.data.credit_note.total, System.Globalization.CultureInfo.InvariantCulture),
            Detalles = factura.Detalles.Select(d => new DetalleNotaCredito
            {
                ProductoId = d.ProductoId,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario
            }).ToList()
        };

        // 6. Devolver stock al inventario
        foreach (var detalle in factura.Detalles)
        {
            var producto = await _productoRepository.ObtenerPorIdsync(detalle.ProductoId);
            if (producto != null)
            {
                int stockAnterior = producto.CantidadStock;
                producto.CantidadStock += detalle.Cantidad;

                // Si el stock era 0 y ahora es mayor que 0, activar el producto
                if (stockAnterior == 0 && producto.CantidadStock > 0)
                {
                    producto.Activo = true;
                }

                await _productoRepository.ActualizarAsync(producto);
            }
        }

        // 7. Guardar en la base de datos
        await _notaCreditoRepository.AgregarAsync(notaCredito);

        // 8. Marcar la factura como anulada
        factura.Anulada = true;
        factura.MotivoAnulacion = factusResponse.data.credit_note.observation;
        factura.FechaAnulacion = DateTime.UtcNow;
        factura.NotaCreditoId = notaCredito.Id;

        // 9. Actualizar la factura en la base de datos
        await _facturaRepository.ActualizarAsync(factura);

        return result;
    }
}