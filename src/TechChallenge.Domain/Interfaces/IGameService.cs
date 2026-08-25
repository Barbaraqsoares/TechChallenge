using TechChallenge.Domain.Exceptions;
using TechChallenge.Domain.Models.Games;

namespace TechChallenge.Domain.Interfaces;
public interface IGameService
{
    Task<IEnumerable<GameResponse>> GetAllAsync();

    /// <summary>Lança <see cref="NotFoundException"/> quando o jogo não existe.</summary>
    Task<GameResponse> GetByIdAsync(int id);

    Task<GameResponse> CreateAsync(CreateGameRequest request);

    /// <summary>Lança <see cref="NotFoundException"/> quando o jogo não existe.</summary>
    Task<GameResponse> UpdateAsync(int id, UpdateGameRequest request);

    /// <summary>Lança <see cref="NotFoundException"/> quando o jogo não existe.</summary>
    Task DeleteAsync(int id);
}
