using TechChallenge.Domain.Entity;

namespace TechChallenge.Domain.Interfaces;

public interface IUserGameService
{
    Task<UserGame> AddGameToLibraryAsync(int userId, int gameId);
    Task<List<UserGame>> GetUserLibraryAsync(int userId);
}