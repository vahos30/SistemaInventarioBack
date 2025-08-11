using MediatR;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
public class FacturasController : ControllerBase
{
    private readonly IMediator _mediator;
    public FacturasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult> CrearFactura([FromBody] CrearFacturaCommand command)
    {
        var resultado = await _mediator.Send(command);
        return CreatedAtAction(nameof(ObtenerFacturaPorId), new { id = resultado.Id }, resultado);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> ObtenerFacturaPorId(Guid id)
    {
        var resultado = await _mediator.Send(new ObtenerFacturaPorIdQuery(id));
        if (resultado == null) return NotFound();
        return Ok(resultado);
    }

    [HttpGet]
    public async Task<ActionResult> ObtenerFacturas()
    {
        var resultado = await _mediator.Send(new ObtenerFacturasQuery());
        return Ok(resultado);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarFactura(Guid id)
    {
        await _mediator.Send(new EliminarFacturaCommand(id));
        return NoContent();
    }

    [HttpPost("{id}/anular")]
    public async Task<IActionResult> AnularFactura(Guid id, [FromBody] string motivo)
    {
        await _mediator.Send(new AnularFacturaCommand { FacturaId = id, Motivo = motivo });
        return NoContent();
    }

    [HttpGet("anuladas")]
    public async Task<IActionResult> GetFacturasAnuladas([FromServices] IMediator mediator)
    {
        var result = await mediator.Send(new ObtenerFacturasAnuladasQuery());
        return Ok(result);
    }
}