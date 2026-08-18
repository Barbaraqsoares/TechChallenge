namespace TechChallenge.Domain.Entity;

/// <summary>
/// Base de todas as entidades: identidade e data de criação.
/// Entidade, no DDD, é o objeto que "tem identidade" — dois usuários com o mesmo
/// nome são pessoas diferentes porque têm Ids diferentes.
/// </summary>
public abstract class EntityBase
{
    public int Id { get; protected set; }

    public DateTime DataCriacao { get; protected set; } = DateTime.UtcNow;
}
