using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TechChallenge.Domain.Entity;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Infrastructure.Authentication;

/// <summary>
/// Gera o token JWT do usuário autenticado.
///
/// O token tem três partes (RFC 7519): header com o algoritmo, payload com as
/// claims e signature. As claims levam quem é o usuário e o seu perfil — é o
/// perfil que o [Authorize(Roles = "Admin")] lê para autorizar cada endpoint.
/// </summary>
public class TokenService : ITokenService
{
    private readonly JwtSettings _configuracoes;

    public TokenService(IOptions<JwtSettings> configuracoes)
    {
        _configuracoes = configuracoes.Value;
    }

    public TokenGerado GerarToken(Usuario usuario)
    {
        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuracoes.SecretKey));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);
        var expiraEm = DateTime.UtcNow.AddMinutes(_configuracoes.ExpiracaoEmMinutos);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, usuario.Email.Endereco),
            new(JwtRegisteredClaimNames.Name, usuario.Nome),

            // Identificador único do token, útil para rastreio e revogação.
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            // É esta claim que habilita a autorização por perfil nos endpoints.
            new(ClaimTypes.Role, usuario.Perfil.ToString()),
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuracoes.Issuer,
            audience: _configuracoes.Audience,
            claims: claims,
            expires: expiraEm,
            signingCredentials: credenciais);

        return new TokenGerado(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiraEm);
    }
}
