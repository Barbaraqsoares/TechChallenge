using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> GetById(int id)
    {
        var promotion = await _promotionService.GetByIdAsync(id);

        if (promotion == null)
            return NotFound("Promoção não encontrada.");

        return Ok(promotion);
    }

    /// <summary>
    /// Cadastra uma nova promoção (apenas para administradores)
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePromotionRequest request)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var adminUserId))
            return Unauthorized();

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
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _promotionService.DeleteAsync(id);

        if (!deleted)
            return NotFound("Promoção não encontrada.");

        return NoContent();
    }
}