using TechChallenge.Domain.Entity;
using TechChallenge.Domain.Exceptions;

namespace TechChallenge.Domain.Interfaces;
public interface IGameService
{
    Task<IEnumerable<Game>> GetAllAsync();

    /// <summary>Lança <see cref="NotFoundException"/> quando o jogo não existe.</summary>
    Task<Game> GetByIdAsync(int id);

    Task<Game> CreateAsync(Game game);

    /// <summary>Lança <see cref="NotFoundException"/> quando o jogo não existe.</summary>
    Task<Game> UpdateAsync(int id, Game game);

    /// <summary>Lança <see cref="NotFoundException"/> quando o jogo não existe.</summary>
    Task DeleteAsync(int id);
}