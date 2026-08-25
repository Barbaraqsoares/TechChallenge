using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TechChallenge.Domain.Entity;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;

    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }
    /// <summary>
    /// Retorna todos os games disponíveis.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Roles = "Admin,Client")]
    public async Task<IActionResult> GetallGames()
    {
        var games = await _gameService.GetAllAsync();

        return Ok(games);
    }

    /// <summary>
    /// Retorna um game específico pelo ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Client")]
    [ProducesResponseType(typeof(Game), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGameById(int id)
    {
        return Ok(await _gameService.GetByIdAsync(id));
    }


    /// <summary>
    /// Cria um novo game (apenas para administradores).
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] Game game)
    {
        var createdGame = await _gameService.CreateAsync(game);

        return CreatedAtAction(nameof(GetGameById), new { id = createdGame.Id }, createdGame);
    }
    /// <summary>
    /// Atualiza um game existente pelo ID (apenas para administradores).
    /// </summary>
    /// <param name="id"></param>
    /// <param name="game"-></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Game), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateGame(int id, [FromBody] Game game)
    {
        return Ok(await _gameService.UpdateAsync(id, game));
    }

    /// <summary>
    /// Deleta um game existente pelo ID (apenas para administradores).
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteGame(int id)
    {
        await _gameService.DeleteAsync(id);

        return NoContent();
    }
}