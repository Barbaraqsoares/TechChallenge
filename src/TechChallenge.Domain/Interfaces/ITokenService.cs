using TechChallenge.Domain.Entity;

namespace TechChallenge.Domain.Interfaces;

/// <summary>
/// Token JWT gerado e o momento em que ele expira.
/// </summary>
public record TokenGerado(string Token, DateTime ExpiraEm);

/// <summary>
/// Gera o token JWT de um usuário autenticado.
/// A interface vive no domínio; a implementação (que conhece a chave secreta e a
/// biblioteca da Microsoft) fica na infraestrutura.
/// </summary>
public interface ITokenService
{
    TokenGerado GerarToken(Usuario usuario);
}
