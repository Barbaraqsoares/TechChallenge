using TechChallenge.Domain.Exceptions;
using TechChallenge.Domain.Models.Promotion;

namespace TechChallenge.Domain.Interfaces;
public interface IPromotionService
{
    Task<IEnumerable<PromotionResponse>> GetAllAsync();

    /// <summary>Lança <see cref="NotFoundException"/> quando a promoção não existe.</summary>
    Task<PromotionResponse> GetByIdAsync(int id);

    Task<PromotionResponse> CreateAsync(CreatePromotionRequest request, int adminUserId);

    /// <summary>Lança <see cref="NotFoundException"/> quando a promoção não existe.</summary>
    Task DeleteAsync(int id);
}