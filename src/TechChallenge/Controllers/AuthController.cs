using Microsoft.AspNetCore.Mvc;
using TechChallenge.Domain.Interfaces;
using TechChallenge.Domain.Models.User;

namespace TechChallenge.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IUserService _userService;

    public AuthController(ITokenService tokenService, IUserService userService)
    {
        _tokenService = tokenService;
        _userService = userService;
    }

    /// <summary>
    /// Registra usuário
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        var createdUser =  await _userService.CreateAsync(request);

        return Created($"/api/users/{createdUser.Id}", createdUser);
    }

    /// <summary>
    /// Login
    /// </summary>
    /// <param name="login"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLogin login)
    {
        var user = await _userService.AuthenticateAsync(login.Login, login.Password);

        if (user == null)
            return Unauthorized("Usuário ou senha inválidos");

        var token = _tokenService.GenerateToken(user);

        return Ok(token);
    } 
}