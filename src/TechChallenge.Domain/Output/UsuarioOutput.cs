using TechChallenge.Domain.Entity;

namespace TechChallenge.Domain.Output;

/// <summary>
/// Espelho do usuário para transitar na API.
///
/// A entidade Usuario nunca é devolvida diretamente: ela carrega o hash da senha
/// e a biblioteca inteira. O DTO expõe apenas o que o cliente precisa ver.
/// </summary>
public class UsuarioOutput
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Perfil { get; set; } = string.Empty;

    public DateTime DataCriacao { get; set; }

    public static UsuarioOutput De(Usuario usuario) => new()
    {
        Id = usuario.Id,
        Nome = usuario.Nome,
        Email = usuario.Email.Endereco,
        Perfil = usuario.Perfil.ToString(),
        DataCriacao = usuario.DataCriacao
    };
}

/// <summary>
/// Resposta do login: o token e os dados de quem entrou.
/// </summary>
public class LoginOutput
{
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiraEm { get; set; }

    public UsuarioOutput Usuario { get; set; } = new();
}
