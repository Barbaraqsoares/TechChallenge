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

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var promotions =
            await _promotionService.GetAllAsync();

        return Ok(promotions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var promotion =
            await _promotionService.GetByIdAsync(id);

        if (promotion == null)
        {
            return NotFound(
                "Promotion not found."
            );
        }

        return Ok(promotion);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePromotionRequest request
    )
    {
        var userIdClaim =
            User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier
            );

        if (userIdClaim == null ||
            !int.TryParse(userIdClaim.Value, out var adminUserId))
        {
            return Unauthorized();
        }

        var promotion =
            await _promotionService.CreateAsync(
                request,
                adminUserId
            );

        return CreatedAtAction(
            nameof(GetById),
            new { id = promotion.Id },
            promotion
        );
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted =
            await _promotionService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(
                "Promotion not found."
            );
        }

        return NoContent();
    }
}