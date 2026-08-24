using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Api.Controllers;

[ApiController]
[Route("api/LibraryOfGames")]
[Authorize]
public class UserGameController : ControllerBase
{
    private readonly IUserGameService _userGameService;

    public UserGameController(IUserGameService userGameService)
    {
        _userGameService = userGameService;
    }

    /// <summary>
    /// Rota para adicionar um jogo à biblioteca do usuário
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="gameId"></param>
    /// <returns></returns>
    [HttpPost("{gameId}")]
    public async Task<IActionResult> AddGameToLibrary( int userId, int gameId)
    {
        var userGame =
            await _userGameService.AddGameToLibraryAsync(userId, gameId);
        return Ok(userGame);
    }

    /// <summary>
    /// Consulta a biblioteca de jogos de um usuário específico
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet()]
    public async Task<IActionResult> GetUserLibrary(int userId)
    {
        var library =
            await _userGameService.GetUserLibraryAsync(userId);
        return Ok(library);
    }
}