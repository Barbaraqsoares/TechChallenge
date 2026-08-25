using TechChallenge.Domain.Entity;
using TechChallenge.Domain.Exceptions;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Domain.Services;

public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;

    public GameService(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task<IEnumerable<Game>> GetAllAsync()
    {
        return await _gameRepository.GetAllAsync();
    }

    public async Task<Game> GetByIdAsync(int id)
    {
        return await _gameRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Jogo {id} não encontrado.");
    }

    public async Task<Game> CreateAsync(Game game)
    {
        ValidateGame(game);

        game.CreatedAt = DateTime.Now;
        game.UpdatedAt = DateTime.Now;
        game.IsActive = true;

        return await _gameRepository.AddAsync(game);
    }

    public async Task<Game> UpdateAsync(int id, Game game)
    {
        var existingGame = await _gameRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Jogo {id} não encontrado.");

        ValidateGame(game);

        existingGame.Name = game.Name;
        existingGame.Description = game.Description;
        existingGame.Price = game.Price;
        existingGame.IsMultiplayer = game.IsMultiplayer;
        existingGame.IsActive = game.IsActive;
        existingGame.UpdatedAt = DateTime.Now;

        await _gameRepository.UpdateAsync(existingGame);

        return existingGame;
    }

    public async Task DeleteAsync(int id)
    {
        var game = await _gameRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Jogo {id} não encontrado.");

        await _gameRepository.DeleteAsync(game);
    }

    private static void ValidateGame(Game game)
    {
        if (string.IsNullOrWhiteSpace(game.Name))
            throw new DomainException("O nome do jogo é obrigatório.");

        if (game.Price < 0)
            throw new DomainException("O preço do jogo não pode ser negativo.");
    }
}