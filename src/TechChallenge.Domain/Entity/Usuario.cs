using TechChallenge.Domain.Exceptions;
using TechChallenge.Domain.ValueObject;

namespace TechChallenge.Domain.Entity;

/// <summary>
/// Usuário da plataforma FIAP Cloud Games.
///
/// É a raiz do agregado Usuário + Biblioteca: os jogos adquiridos só podem ser
/// alterados pelos métodos desta classe (consistência forçada do DDD), nunca
/// manipulando a coleção diretamente de fora.
/// </summary>
public class Usuario : EntityBase
{
    private readonly List<UsuarioJogo> _biblioteca = [];

    /// <summary>
    /// Construtor sem parâmetros exigido pelo Entity Framework na leitura do banco.
    /// </summary>
    private Usuario()
    {
    }

    private Usuario(string nome, Email email, Senha senha, PerfilEnum perfil)
    {
        Nome = nome;
        Email = email;
        Senha = senha;
        Perfil = perfil;
    }

    public string Nome { get; private set; } = string.Empty;

    public Email Email { get; private set; } = null!;

    public Senha Senha { get; private set; } = null!;

    /// <summary>
    /// Nível de acesso: Cliente (usa a plataforma) ou Admin (administra).
    /// </summary>
    public PerfilEnum Perfil { get; private set; }

    /// <summary>
    /// Biblioteca de jogos adquiridos. Exposta como somente leitura: quem quiser
    /// incluir um jogo precisa usar AdquirirJogo.
    /// </summary>
    public IReadOnlyCollection<UsuarioJogo> Biblioteca => _biblioteca.AsReadOnly();

    /// <summary>
    /// Cria um usuário validando nome, e-mail e senha.
    /// Por padrão nasce com perfil Cliente — administradores são criados
    /// explicitamente por outro administrador.
    /// </summary>
    public static Usuario Criar(
        string nome,
        string email,
        string senha,
        PerfilEnum perfil = PerfilEnum.Cliente)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException("O nome é obrigatório.");
        }

        return new Usuario(
            nome.Trim(),
            Email.Criar(email),
            Senha.Criar(senha),
            perfil);
    }

    /// <summary>
    /// Confere as credenciais informadas no login.
    /// </summary>
    public bool Autenticar(string senhaEmTextoPuro) => Senha.Conferir(senhaEmTextoPuro);

    public void AlterarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException("O nome é obrigatório.");
        }

        Nome = nome.Trim();
    }

    public void AlterarEmail(string email) => Email = Email.Criar(email);

    public void AlterarSenha(string novaSenha) => Senha = Senha.Criar(novaSenha);

    /// <summary>
    /// Troca o nível de acesso do usuário. Operação restrita a administradores
    /// (a verificação do perfil de quem chama é feita na camada de API).
    /// </summary>
    public void AlterarPerfil(PerfilEnum perfil) => Perfil = perfil;

    /// <summary>
    /// Adiciona um jogo à biblioteca do usuário pelo preço efetivamente pago
    /// (que pode ser menor que o preço de tabela, quando há promoção).
    /// </summary>
    public UsuarioJogo AdquirirJogo(Jogo jogo)
    {
        ArgumentNullException.ThrowIfNull(jogo);

        if (JaPossui(jogo.Id))
        {
            throw new DomainException($"O jogo '{jogo.Titulo}' já está na sua biblioteca.");
        }

        var aquisicao = UsuarioJogo.Criar(this, jogo, jogo.PrecoAtual());
        _biblioteca.Add(aquisicao);

        return aquisicao;
    }

    public bool JaPossui(int jogoId) => _biblioteca.Any(item => item.JogoId == jogoId);
}
