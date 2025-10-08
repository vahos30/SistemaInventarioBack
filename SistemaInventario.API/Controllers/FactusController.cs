using Microsoft.AspNetCore.Mvc;
using MediatR;
using SistemaInventario.Application.Feactures.Facturas;
using SistemaInventario.Application.Services; 


[ApiController]
[Route("api/[controller]")]
public class FactusController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly FactusFacturaService _factusFacturaService;

    public FactusController(IMediator mediator, FactusFacturaService factusFacturaService)
    {
        _mediator = mediator;
        _factusFacturaService = factusFacturaService;
    }

    [HttpPost("crear-factura-factus")]
    public async Task<IActionResult> CrearFacturaFactus([FromBody] CrearFacturaFactusCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("descargar-factura-pdf/{numeroFactura}")]
    public async Task<IActionResult> DescargarFacturaPdf(string numeroFactura)
    {
        var (fileName, pdfBase64) = await _factusFacturaService.DescargarFacturaPdfAsync(numeroFactura);
        var pdfBytes = Convert.FromBase64String(pdfBase64);
        return File(pdfBytes, "application/pdf", $"{fileName}.pdf");
    }

    [HttpPost("crear-nota-credito-factus")]
    public async Task<IActionResult> CrearNotaCreditoFactus([FromBody] CrearNotaCreditoFactusCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}