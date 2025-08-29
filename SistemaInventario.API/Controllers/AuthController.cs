using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SistemaInventario.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<Usuario> _userManager;
    private readonly SignInManager<Usuario> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var user = await _userManager.FindByNameAsync(loginDto.NombreUsuario);
        if (user == null) return Unauthorized();

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Contrasena, false);
        if (!result.Succeeded) return Unauthorized();

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id), // <-- AGREGAR ESTE CLAIM
            new Claim(ClaimTypes.Name, user.UserName)
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(8),
            signingCredentials: creds);

        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    }

    // 1. Solicitar token de recuperación
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return BadRequest("Usuario no encontrado.");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        // Para pruebas, devuelve el token en la respuesta.
        // En producción, deberías enviarlo por email.
        return Ok(new { token });
    }

    // 2. Restablecer la contraseña
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return BadRequest("Usuario no encontrado.");

        var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("Contraseña restablecida correctamente.");
    }
}