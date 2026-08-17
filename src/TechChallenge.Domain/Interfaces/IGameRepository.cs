using TechChallenge.Domain.Entity;

namespace TechChallenge.Domain.Interfaces;

public interface IGameRepository
{
    Task<IEnumerable<Game>> GetAllAsync();

    Task<Game?> GetByIdAsync(int id);

    Task<Game> AddAsync(Game game);

    Task UpdateAsync(Game game);

    Task DeleteAsync(Game game);
}