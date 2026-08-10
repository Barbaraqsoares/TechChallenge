using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TechChallenge.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] UsuarioLogin login)
    {
        // Exemplo simples: valida usuários fixos
        if (login.Username == "joao" && login.Password == "1234")
        {
            var token = GerarToken(login.Username, "User");
            return Ok(new { token });
        }
        else if (login.Username == "maria" && login.Password == "abcd")
        {
            var token = GerarToken(login.Username, "Admin");
            return Ok(new { token });
        }

        return Unauthorized("Usuário ou senha inválidos");
    }

    private string GerarToken(string username, string role)
    {
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);
        var tokenHandler = new JwtSecurityTokenHandler();

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role) // Aqui definimos User ou Admin
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = "suaempresa.com",
            Audience = "suaempresa.com",
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

public class UsuarioLogin
{
    public string Username { get; set; }
    public string Password { get; set; }
}