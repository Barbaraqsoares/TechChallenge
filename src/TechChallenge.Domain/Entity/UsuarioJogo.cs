namespace TechChallenge.Domain.Entity;

/// <summary>
/// Registro de um jogo adquirido por um usuário — cada item da biblioteca.
///
/// Guarda o preço efetivamente pago, e não apenas a referência ao jogo: se o
/// preço do catálogo mudar depois, o histórico da compra continua correto.
/// </summary>
public class UsuarioJogo : EntityBase
{
    /// <summary>
    /// Construtor sem parâmetros exigido pelo Entity Framework na leitura do banco.
    /// </summary>
    private UsuarioJogo()
    {
    }

    private UsuarioJogo(Usuario usuario, Jogo jogo, decimal precoPago)
    {
        Usuario = usuario;
        UsuarioId = usuario.Id;
        Jogo = jogo;
        JogoId = jogo.Id;
        PrecoPago = precoPago;
        DataAquisicao = DateTime.UtcNow;
    }

    public int UsuarioId { get; private set; }

    public int JogoId { get; private set; }

    /// <summary>
    /// Quanto o usuário pagou no momento da compra.
    /// </summary>
    public decimal PrecoPago { get; private set; }

    public DateTime DataAquisicao { get; private set; }

    // Propriedades de navegação usadas pelo Entity Framework nos Includes.
    public Usuario Usuario { get; private set; } = null!;

    public Jogo Jogo { get; private set; } = null!;

    /// <summary>
    /// Criado apenas pelo agregado Usuario, através de Usuario.AdquirirJogo.
    /// </summary>
    internal static UsuarioJogo Criar(Usuario usuario, Jogo jogo, decimal precoPago) =>
        new(usuario, jogo, precoPago);
}
