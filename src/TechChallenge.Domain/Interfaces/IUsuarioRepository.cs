using TechChallenge.Domain.Entity;

namespace TechChallenge.Domain.Interfaces;

public interface IUsuarioRepository : IRepository<Usuario>
{
    /// <summary>
    /// Busca pelo e-mail — usado no login e para impedir cadastro duplicado.
    /// </summary>
    Task<Usuario?> ObterPorEmail(string email);

    /// <summary>
    /// Retorna o usuário com a biblioteca de jogos carregada (eager loading),
    /// evitando o problema N+1 de uma consulta por jogo.
    /// </summary>
    Task<Usuario?> ObterComBiblioteca(int id);

    Task<bool> ExisteComEmail(string email);
}
