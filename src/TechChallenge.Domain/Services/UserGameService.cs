using TechChallenge.Domain.Entity;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Domain.Services;

public class UserGameService : IUserGameService
{
    private readonly IUserGameRepository _userGameRepository;
    private readonly IUserRepository _userRepository;
    private readonly IGameRepository _gameRepository;

    public UserGameService(
        IUserGameRepository userGameRepository,
        IUserRepository userRepository,
        IGameRepository gameRepository
    )
    {
        _userGameRepository = userGameRepository;
        _userRepository = userRepository;
        _gameRepository = gameRepository;
    }

    public async Task<UserGame> AddGameToLibraryAsync(int userId,int gameId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            throw new InvalidOperationException(
                "Usuário não encontrado."
            );
        }

        var game = await _gameRepository.GetByIdAsync(gameId);

        if (game == null)
        {
            throw new InvalidOperationException(
                "Jogo não encontrado."
            );
        }

        var existingUserGame =
            await _userGameRepository.GetByUserAndGameAsync(
                userId,
                gameId
            );

        if (existingUserGame != null)
        {
            throw new InvalidOperationException(
                "O jogo já está na biblioteca do usuário."
            );
        }

        var userGame = new UserGame
        {
            UserId = userId,
            GameId = gameId,
            PurchasedAt = DateTime.UtcNow
        };

        return await _userGameRepository.AddAsync(userGame);
    }

    public async Task<List<UserGame>> GetUserLibraryAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            throw new InvalidOperationException(
                "Usuário não encontrado."
            );
        }

        return await _userGameRepository.GetByUserIdAsync(userId);
    }
}