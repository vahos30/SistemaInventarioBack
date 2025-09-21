using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CiudadController : ControllerBase
{
    private readonly CiudadService _ciudadService;

    public CiudadController(CiudadService ciudadService)
    {
        _ciudadService = ciudadService;
    }

    [HttpGet("departamentos")]
    public async Task<IActionResult> GetDepartamentosConCiudades()
    {
        var ciudades = await _ciudadService.ObtenerCiudadesAsync();
        var agrupado = ciudades
            .GroupBy(c => c.department)
            .ToDictionary(g => g.Key, g => g.ToList());
        return Ok(agrupado);
    }
}