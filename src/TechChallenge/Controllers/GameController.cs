using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TechChallenge.Domain.Entity;
using TechChallenge.Domain.Interfaces;
using TechChallenge.Domain.Services;

namespace TechChallenge.Controllers;

[ApiController]
[Route("[controller]")]
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
    public async Task<IActionResult> GetGameById(int id)
    {
        var game = await _gameService.GetByIdAsync(id);

        if (game == null)
        {
            return NotFound("Jogo não encontrado.");
        }

        return Ok(game);
    }


    /// <summary>
    /// Cria um novo game.
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Game game)
    {
        var createdGame = await _gameService.CreateAsync(game);

        return CreatedAtAction(
            nameof(GetGameById),
            new { id = createdGame.Id },
            createdGame
        );
    }
    /// <summary>
    /// Atualiza um game existente pelo ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGame(
        int id,
        [FromBody] Game game
    )
    {
        var updatedGame =
            await _gameService.UpdateAsync(id, game);

        if (updatedGame == null)
        {
            return NotFound("Jogo não encontrado.");
        }

        return Ok(updatedGame);
    }

    /// <summary>
    /// Deleta um game existente pelo ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGame(int id)
    {
        var deleted =
            await _gameService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound("Jogo não encontrado.");
        }

        return NoContent();
    }
}
