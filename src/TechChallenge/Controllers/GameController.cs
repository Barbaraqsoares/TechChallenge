using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TechChallenge.Domain.Interfaces;
using TechChallenge.Domain.Models.Games;

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
    [HttpGet]
    [Authorize(Roles = "Admin,Client")]
    [ProducesResponseType(typeof(IEnumerable<GameResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetallGames()
    {
        var games = await _gameService.GetAllAsync();

        return Ok(games);
    }

    /// <summary>
    /// Retorna um game específico pelo ID.
    /// </summary>
    /// <param name="id">Identificador do game.</param>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Client")]
    [ProducesResponseType(typeof(GameResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGameById(int id)
    {
        return Ok(await _gameService.GetByIdAsync(id));
    }

    /// <summary>
    /// Cria um novo game (apenas para administradores).
    /// </summary>
    /// <param name="request">Nome, descrição, preço e se é multiplayer.</param>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(GameResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateGameRequest request)
    {
        var createdGame = await _gameService.CreateAsync(request);

        return CreatedAtAction(nameof(GetGameById), new { id = createdGame.Id }, createdGame);
    }

    /// <summary>
    /// Atualiza um game existente pelo ID (apenas para administradores).
    /// </summary>
    /// <param name="id">Identificador do game.</param>
    /// <param name="request">Dados do game, incluindo se continua ativo no catálogo.</param>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(GameResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateGame(int id, [FromBody] UpdateGameRequest request)
    {
        return Ok(await _gameService.UpdateAsync(id, request));
    }

    /// <summary>
    /// Deleta um game existente pelo ID (apenas para administradores).
    /// </summary>
    /// <param name="id">Identificador do game.</param>
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
