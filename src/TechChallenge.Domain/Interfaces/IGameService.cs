using TechChallenge.Domain.Entity;

namespace TechChallenge.Domain.Interfaces;

public interface IGameService
{
    Task<IEnumerable<Game>> GetAllAsync();
    Task<Game?> GetByIdAsync(int id);
    Task<Game> CreateAsync(Game game);
    Task<Game?> UpdateAsync(int id, Game game);
    Task<bool> DeleteAsync(int id);
}