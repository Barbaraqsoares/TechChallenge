using TechChallenge.Domain.Entity;

namespace TechChallenge.Domain.Interfaces;

public interface IUserGameRepository
{
    Task<UserGame?> GetByUserAndGameAsync(int userId, int gameId);
    Task<List<UserGame>> GetByUserIdAsync(int userId);
    Task<UserGame> AddAsync(UserGame userGame);
}