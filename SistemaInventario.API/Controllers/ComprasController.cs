using Microsoft.AspNetCore.Mvc;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Application.Services;


namespace SistemaInventario.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompraController : ControllerBase
    {
        private readonly CompraService _compraService;

        public CompraController(CompraService compraService)
        {
            _compraService = compraService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var compras = await _compraService.ObtenerTodasAsync();
            return Ok(compras);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var compra = await _compraService.ObtenerPorIdAsync(id);
            if (compra == null)
                return NotFound();
            return Ok(compra);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CompraCreateDto compraDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Mapea el DTO a tu entidad Compra y Detalles
            var compra = new Compra
            {
                ProveedorId = compraDto.ProveedorId,
                Detalles = compraDto.Detalles.Select(d => new DetalleCompra
                {
                    ProductoId = d.ProductoId,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario
                }).ToList()
            };

            await _compraService.CrearAsync(compra);
            return Ok(compra);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Compra compra)
        {
            if (id != compra.Id)
                return BadRequest("ID mismatch");

            await _compraService.ActualizarAsync(compra);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _compraService.EliminarAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/anular-parcial")]
        public async Task<IActionResult> AnularParcial(Guid id, [FromBody] CompraAnulacionParcialDto dto)
        {
            if (id != dto.CompraId)
                return BadRequest("ID mismatch");

            await _compraService.AnularParcialAsync(dto);
            return NoContent();
        }
    }
}
