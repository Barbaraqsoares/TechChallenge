namespace TechChallenge.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando o recurso solicitado não existe.
/// Exemplo: "Jogo 42 não encontrado".
/// O middleware de erros traduz esta exceção em HTTP 404 (Not Found).
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string message) : base(message)
    {
    }
}
