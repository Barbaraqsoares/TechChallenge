using TechChallenge.Domain.Entity;

namespace TechChallenge.Domain.Interfaces;

public record GeneratedToken(string Token, DateTime ExpiraEm);
public interface ITokenService
{
    GeneratedToken GenerateToken(User usuario);
}
