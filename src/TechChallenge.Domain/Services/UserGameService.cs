using TechChallenge.Domain.Entity;
using TechChallenge.Domain.Interfaces;
using TechChallenge.Domain.Exceptions;
using TechChallenge.Domain.Models.UserGame;


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

    public async Task<UserGameResponse> AddGameToLibraryAsync(int userId,int gameId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            throw new NotFoundException("Usuário não encontrado.");

        var game = await _gameRepository.GetByIdAsync(gameId);

        if (game == null)
            throw new NotFoundException("Jogo não encontrado.");

        var existingUserGame = await _userGameRepository.GetByUserAndGameAsync( userId, gameId);

        if (existingUserGame != null)
            throw new InvalidOperationException("O jogo já está na biblioteca do usuário.");

        var userGame = new UserGame
        {
            UserId = userId,
            GameId = gameId,
            PurchasedAt = DateTime.Now
        };

        var createdUserGame = await _userGameRepository.AddAsync(userGame);

        return new UserGameResponse
        {
            GameId = game.Id,
            GameName = game.Name,
            Price = game.Price,
            PurchasedAt = createdUserGame.PurchasedAt
        };
    }

    public async Task<List<UserGameResponse>> GetUserLibraryAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            throw new NotFoundException("Usuário não encontrado.");

        var userGames = await _userGameRepository.GetByUserIdAsync(userId);

        return userGames.Select(userGame => new UserGameResponse()
        {
            GameId = userGame.GameId,
            GameName = userGame.Game.Name,
            Price = userGame.Game.Price,
            PurchasedAt = userGame.PurchasedAt
        }).ToList();
    }

    Task<UserGameResponse> IUserGameService.AddGameToLibraryAsync(int userId, int gameId)
    {
        throw new NotImplementedException();
    }

    Task<List<UserGameResponse>> IUserGameService.GetUserLibraryAsync(int userId)
    {
        throw new NotImplementedException();
    }
}