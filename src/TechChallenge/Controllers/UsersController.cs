using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Consulta todos os usuários cadastrados (apenas para administradores)
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _userService.GetAllAsync());
    }

    /// <summary>
    /// Consulta um usuário específico pelo ID (apenas para administradores)
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);

        return user == null ? NotFound() : Ok(user);
    }

    /// <summary>
    /// Deleta um usuário específico pelo ID (apenas para administradores)
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _userService.DeleteAsync(id);

        return deleted ? NoContent() : NotFound();
    }
}