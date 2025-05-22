using Microsoft.AspNetCore.Mvc;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Application.Services;

namespace SistemaInventario.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProveedorController : ControllerBase
    {
        private readonly ProveedorService _proveedorService;

        public ProveedorController(ProveedorService proveedorService)
        {
            _proveedorService = proveedorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var proveedores = await _proveedorService.ObtenerTodosAsync();
            return Ok(proveedores);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var proveedor = await _proveedorService.ObtenerPorIdAsync(id);
            if (proveedor == null)
                return NotFound();
            return Ok(proveedor);
        }

        [HttpGet("nit/{nit}")]
        public async Task<IActionResult> GetByNIT(string nit)
        {
            var proveedor = await _proveedorService.ObtenerPorNitAsync(nit);
            if (proveedor == null)
                return NotFound();
            return Ok(proveedor);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Proveedor proveedor)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _proveedorService.CrearAsync(proveedor);
            return CreatedAtAction(nameof(GetById), new { id = proveedor.Id }, proveedor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Proveedor proveedor)
        {
            if (id != proveedor.Id)
                return BadRequest("ID mismatch");

            await _proveedorService.ActualizarAsync(proveedor);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _proveedorService.EliminarAsync(id);
            return NoContent();
        }
    }
}
