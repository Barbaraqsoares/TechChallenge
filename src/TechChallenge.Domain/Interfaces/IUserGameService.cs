namespace TechChallenge.Domain.Interfaces;

public interface IUserGameService
{
    Task<UserGameResponse> AddGameToLibraryAsync(int userId, int gameId);
    Task<List<UserGameResponse>> GetUserLibraryAsync(int userId);
}