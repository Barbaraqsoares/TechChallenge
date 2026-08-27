using TechChallenge.Domain.Entity;

namespace TechChallenge.Domain.Interfaces;
public interface IPromotionRepository
{
    Task<IEnumerable<Promotion>> GetAllAsync();
    Task<Promotion?> GetByIdAsync(int id);
    Task<Promotion> AddAsync(Promotion Promotion);
    Task UpdateAsync(Promotion Promotion);
    Task DeleteAsync(Promotion Promotion);
}
