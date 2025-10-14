using MediatR;
using System;
using System.Collections.Generic;

public class CrearNotaCreditoFactusCommand : IRequest<string>
{
    public int CorrectionConceptCode { get; set; }
    public int CustomizationId { get; set; }
    public string PaymentMethodCode { get; set; }
    public string Observation { get; set; }
    public Guid FacturaId { get; set; } // Id de la factura interna a referenciar
    // Puedes agregar más campos si lo necesitas
}