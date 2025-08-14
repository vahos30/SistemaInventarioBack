using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.Application.Feactures.Recibos;
using SistemaInventario.Application.Feactures.Reportes;  
using System;
using System.Threading.Tasks;

namespace SistemaInventario.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Obtiene el inventario actual (lista de productos con stock y precio).
        /// Endpoint: GET /api/reportes/inventario
        /// </summary>
        [HttpGet("inventario")]
        public async Task<IActionResult> ObtenerInventario()
        {
            var query = new ObtenerInventarioQuery();
            var inventario = await _mediator.Send(query);
            return Ok(inventario);
        }

        /// <summary>
        /// Obtiene todos los recibos y facturas (ventas) asociados a un cliente específico.
        /// Endpoint: GET /api/reportes/cliente/{clienteId}
        /// </summary>
        /// <param name="clienteId">Identificador del cliente.</param>
        [HttpGet("cliente/{clienteId}")]
        public async Task<IActionResult> ObtenerVentasPorCliente(Guid clienteId)
        {
            var query = new ObtenerVentasPorClienteQuery(clienteId);
            var ventas = await _mediator.Send(query);
            return Ok(ventas);
        }

        /// <summary>
        /// Obtiene las ventas (recibos) del día actual.
        /// Endpoint: GET /api/reportes/ventas-diarias
        /// </summary>
        [HttpGet("ventas-diarias")]
        public async Task<IActionResult> ObtenerVentasDiarias()
        {
            var query = new ObtenerVentasDiariasQuery();
            var ventasDiarias = await _mediator.Send(query);
            return Ok(ventasDiarias);
        }

        /// <summary>
        /// Obtiene las ventas (recibos) en un rango de fechas.
        /// Endpoint: GET /api/reportes/ventas?fechaInicio=YYYY-MM-DD&fechaFin=YYYY-MM-DD
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio.</param>
        /// <param name="fechaFin">Fecha final.</param>
        [HttpGet("ventas")]
        public async Task<IActionResult> ObtenerVentasPorFechas([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            var query = new ObtenerVentasPorFechasQuery(fechaInicio, fechaFin);
            var ventas = await _mediator.Send(query);
            return Ok(ventas);
        }

        /// <summary>
        /// Obtiene las compras realizadas en un rango de fechas (o todas si no pasas fechas).
        /// Endpoint: GET /api/reportes/compras?fechaInicio=YYYY-MM-DD&fechaFin=YYYY-MM-DD
        /// </summary>
        [HttpGet("compras")]
        public async Task<IActionResult> ObtenerComprasPorFechas([FromQuery] DateTime? fechaInicio, [FromQuery] DateTime? fechaFin)
        {
            var query = new ObtenerComprasPorFechasQuery(fechaInicio, fechaFin);
            var compras = await _mediator.Send(query);
            return Ok(compras);
        }
    }
}

