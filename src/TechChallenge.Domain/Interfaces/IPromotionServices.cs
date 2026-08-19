using TechChallenge.Domain.Models.Promotion;

namespace TechChallenge.Domain.Interfaces;

public interface IPromotionService
{
    Task<IEnumerable<PromotionResponse>> GetAllAsync();
    Task<PromotionResponse?> GetByIdAsync(int id);
    Task<PromotionResponse> CreateAsync(CreatePromotionRequest request, int adminUserId);
    Task<bool> DeleteAsync(int id);
}