using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.Application.Feactures.Clientes;
using SistemaInventario.Application.Feactures.Recibos;

namespace SistemaInventario.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecibosController : ControllerBase
    {
        private readonly IMediator _mediator;
        public RecibosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // crear un recibo
        [HttpPost]
        public async Task<ActionResult> CrearRecibo([FromBody] CrearClienteCommand command)
        {
            var resultado = await _mediator.Send(command);
            return CreatedAtAction(nameof(obtenerReciboPorId), new { id = resultado.Id }, resultado);
        }

        // obtener un recibo por su Id
        [HttpGet("{id}")]
        public async Task<ActionResult> obtenerReciboPorId(Guid id)
        {
            var resultado = await _mediator.Send(new ObtenerReciboPorIdQuery(id));
            if (resultado == null) return NotFound();
            return Ok(resultado);

        }

        // obtener todos los recibos
        [HttpGet]
        public async Task<ActionResult> obtenerRecibos()
        {
            var resultado = await _mediator.Send(new ObtenerRecibosQuery());
            return Ok(resultado);
        }
    }
}
