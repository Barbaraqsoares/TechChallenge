using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TechChallenge.Domain.Entity;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;

    public AuthController(IConfiguration configuration, IUserService userService)
    {
        _configuration = configuration;
        _userService = userService;
    }

    /// <summary>
    /// Login
    /// </summary>
    /// <param name="login"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UsuarioLogin login)
    {
        var user = await _userService.AuthenticateAsync(login.Login, login.Password);

        if (user == null)
        {
            return Unauthorized("Usuário ou senha inválidos");
        }

        var token = GerarToken(user.Login, user.Perfil.ToString());

        return Ok(new { token });
    }

    /// <summary>
    /// Registra usuário
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register(
    [FromBody] User user
)
    {
        var createdUser =
            await _userService.CreateAsync(user);

        return Created(
            $"/api/users/{createdUser.Id}",
            createdUser
        );
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
    public string Login { get; set; }
    public string Password { get; set; }
}