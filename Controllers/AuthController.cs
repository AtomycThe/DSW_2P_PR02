using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DSW_3P_PR02.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DSW_3P_PR02.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [EndpointSummary("Iniciar sesión con el usuario de demostración")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] LoginDto login)
    {
        // Usuario de demostración (la contraseña está en User Secrets)
        var demo = _configuration.GetSection("DemoUser");
        if (login.Usuario != demo["Usuario"] ||
            string.IsNullOrEmpty(demo["Password"]) ||
            login.Password != demo["Password"])
        {
            return Unauthorized(new { mensaje = "Credenciales incorrectas" });
        }

        var jwt = _configuration.GetSection("Jwt");
        var key = Convert.FromBase64String(
            jwt["Key"] ?? throw new InvalidOperationException("No se configuró Jwt:Key"));
        int minutos = int.Parse(jwt["ExpirationMinutes"] ?? "30");

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, login.Usuario),
            new Claim(ClaimTypes.Role, "Administrador")
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(minutos),
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new
        {
            access_token = tokenString,
            token_type = "Bearer",
            expires_in = minutos * 60
        });
    }
}
