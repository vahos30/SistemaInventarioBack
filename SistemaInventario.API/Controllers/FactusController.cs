using Microsoft.AspNetCore.Mvc;
using MediatR;
using SistemaInventario.Application.Feactures.Facturas;


[ApiController]
[Route("api/[controller]")]
public class FactusController : ControllerBase
{
    private readonly IMediator _mediator;

    public FactusController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("crear-factura-factus")]
    public async Task<IActionResult> CrearFacturaFactus([FromBody] CrearFacturaFactusCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}