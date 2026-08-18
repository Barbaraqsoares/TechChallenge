using TechChallenge.Domain.Entity;

namespace TechChallenge.Domain.Interfaces;

/// <summary>
/// Repositório genérico: abstrai o acesso a dados para que a regra de negócio
/// não dependa do banco, e para que os testes possam usar um substituto (mock).
/// </summary>
public interface IRepository<T> where T : EntityBase
{
    Task<IList<T>> ObterTodos();

    Task<T?> ObterPorId(int id);

    Task Cadastrar(T entidade);

    Task Alterar(T entidade);

    Task Deletar(int id);
}
