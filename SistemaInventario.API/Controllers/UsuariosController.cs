using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.Application.DTOs;
using SistemaInventario.Domain.Entities;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly UserManager<Usuario> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsuariosController(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // Solo uso interno, no exponer al frontend
    [HttpPost("crear-administrador")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> CrearAdministrador([FromBody] RegistrarUsuarioDto dto)
    {
        if (!await _roleManager.RoleExistsAsync("Administrador"))
            await _roleManager.CreateAsync(new IdentityRole("Administrador"));

        var user = new Usuario { UserName = dto.UserName, Email = dto.Email };
        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(user, "Administrador");
        return Ok("Administrador creado correctamente");
    }

    // Expuesta al frontend, solo administradores pueden crear vendedores
    [HttpPost("crear-vendedor")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> CrearVendedor([FromBody] RegistrarUsuarioDto dto)
    {
        if (!await _roleManager.RoleExistsAsync("Vendedor"))
            await _roleManager.CreateAsync(new IdentityRole("Vendedor"));

        var user = new Usuario { UserName = dto.UserName, Email = dto.Email };
        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(user, "Vendedor");
        return Ok("Vendedor creado correctamente");
    }

    // Endpoint para listar usuarios (solo administradores)
    [HttpGet("listar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ListarUsuarios()
    {
        var usuarios = _userManager.Users
            .Select(u => new
            {
                u.Id,
                u.UserName,
                u.Email
            })
            .ToList();

        // Opcional: incluir roles de cada usuario
        var usuariosConRoles = new List<object>();
        foreach (var usuario in usuarios)
        {
            var user = await _userManager.FindByIdAsync(usuario.Id);
            var roles = await _userManager.GetRolesAsync(user);
            usuariosConRoles.Add(new
            {
                usuario.Id,
                usuario.UserName,
                usuario.Email,
                Roles = roles
            });
        }

        return Ok(usuariosConRoles);
    }

    [HttpGet("mi-perfil")]
    [Authorize]
    public async Task<IActionResult> MiPerfil()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound("Usuario no encontrado.");

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(new {
            user.Id,
            user.UserName,
            user.Email,
            Roles = roles
        });
    }
}