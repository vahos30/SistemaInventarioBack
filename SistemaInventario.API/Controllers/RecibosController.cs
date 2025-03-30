using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.Application.Feactures.Clientes;
using SistemaInventario.Application.Feactures.Productos;
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
        public async Task<ActionResult> CrearRecibo([FromBody] CrearReciboCommand command)
        {
            // Validar existencia de cliente y productos
            var clienteExiste = await _mediator.Send(new ExisteClienteQuery(command.ClienteId));
            if (!clienteExiste) return BadRequest("Cliente no encontrado");

            foreach (var detalle in command.Detalles)
            {
                var productoExiste = await _mediator.Send(new ExisteProductoQuery(detalle.ProductoId));
                if (!productoExiste) return BadRequest($"Producto {detalle.ProductoId} no existe");
            }

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
