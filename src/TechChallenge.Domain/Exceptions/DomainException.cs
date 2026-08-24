namespace TechChallenge.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando uma regra de negócio é violada.
/// Exemplo: "A senha deve conter no mínimo 8 caracteres".
/// O middleware de erros traduz esta exceção em HTTP 400 (Bad Request).
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
