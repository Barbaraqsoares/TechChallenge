using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechChallenge.Domain.Interfaces;
using TechChallenge.Domain.Models.Promotion;

namespace TechChallenge.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PromotionsController : ControllerBase
{
    private readonly IPromotionService _promotionService;

    public PromotionsController(IPromotionService promotionService)
    {
        _promotionService = promotionService;
    }
    /// <summary>
    /// Consulta promoções disponíveis
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var promotions = await _promotionService.GetAllAsync();

        return Ok(promotions);
    }

    /// <summary>
    /// Consulta uma promoção específica pelo ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PromotionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        return Ok(await _promotionService.GetByIdAsync(id));
    }

    /// <summary>
    /// Cadastra uma nova promoção (apenas para administradores)
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(typeof(PromotionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreatePromotionRequest request)
    {
        var adminUserId = GetAuthenticatedUserId();

        var promotion = await _promotionService.CreateAsync(request, adminUserId);

        return CreatedAtAction(nameof(GetById), new { id = promotion.Id }, promotion);
    }

    /// <summary>
    /// Deleta uma promoção pelo ID (apenas para administradores)
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _promotionService.DeleteAsync(id);

        return NoContent();
    }

    /// <summary>
    /// Extrai o id do usuário autenticado do token. A claim é obrigatória: sem ela
    /// não há como registrar quem criou a promoção.
    /// </summary>
    private int GetAuthenticatedUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            throw new UnauthorizedAccessException("Usuário inválido.");

        return userId;
    }
}