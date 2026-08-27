using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechChallenge.Domain.Interfaces;
using TechChallenge.Domain.Models.UserGame;

namespace TechChallenge.Api.Controllers;

[ApiController]
[Route("api/LibraryOfGames")]
[Authorize]
public class UserGameController : ControllerBase
{
    private readonly IUserGameService _userGameService;

    public UserGameController(
        IUserGameService userGameService)
    {
        _userGameService = userGameService;
    }

    /// <summary>
    /// Adiciona um jogo à biblioteca
    /// do usuário autenticado.
    /// </summary>
    [HttpPost("{gameId}")]
    [ProducesResponseType(typeof(UserGameResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddGameToLibrary(int gameId)
    {
        var userId = GetAuthenticatedUserId();

        var userGame = await _userGameService.AddGameToLibraryAsync(userId,gameId);

        return Ok(userGame);
    }

    /// <summary>
    /// Retorna a biblioteca do usuário autenticado.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<UserGameResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserLibrary()
    {
        var userId = GetAuthenticatedUserId();

        var library = await _userGameService.GetUserLibraryAsync(userId);

        return Ok(library);
    }

    private int GetAuthenticatedUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value,out var userId))
            throw new UnauthorizedAccessException("Usuário inválido.");

        return userId;
    }
}