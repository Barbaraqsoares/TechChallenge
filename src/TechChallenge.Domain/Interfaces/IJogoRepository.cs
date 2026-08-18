using TechChallenge.Domain.Entity;

namespace TechChallenge.Domain.Interfaces;

public interface IJogoRepository : IRepository<Jogo>
{
    /// <summary>
    /// Jogos com promoção vigente (desconto maior que zero).
    /// </summary>
    Task<IList<Jogo>> ObterEmPromocao();

    Task<IList<Jogo>> ObterPorGenero(string genero);
}
