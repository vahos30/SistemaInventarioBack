using MediatR;
using SistemaInventario.Domain.Interfaces;
using SistemaInventario.Application.Services;
using SistemaInventario.Application.Mappers;
using System.Threading;
using System.Threading.Tasks;

public class CrearNotaCreditoFactusCommandHandler : IRequestHandler<CrearNotaCreditoFactusCommand, string>
{
    private readonly IFacturaRepository _facturaRepository;
    private readonly FactusFacturaService _factusFacturaService;

    public CrearNotaCreditoFactusCommandHandler(
        IFacturaRepository facturaRepository,
        FactusFacturaService factusFacturaService)
    {
        _facturaRepository = facturaRepository;
        _factusFacturaService = factusFacturaService;
    }

    public async Task<string> Handle(CrearNotaCreditoFactusCommand request, CancellationToken cancellationToken)
    {
        // 1. Obtener la factura original
        var factura = await _facturaRepository.ObtenerPorIdAsync(request.FacturaId);
        if (factura == null)
            throw new Exception("Factura no encontrada.");

        // 2. Mapear los datos de la factura a la estructura de la nota crédito
        var notaCreditoRequest = new
        {
            numbering_range_id = 756,
            correction_concept_code = request.CorrectionConceptCode,
            customization_id = request.CustomizationId,
            bill_id = request.BillId,
            reference_code = request.ReferenceCode,
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
        return result;
    }
}