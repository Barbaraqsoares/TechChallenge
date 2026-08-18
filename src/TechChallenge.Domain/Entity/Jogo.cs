using TechChallenge.Domain.Exceptions;

namespace TechChallenge.Domain.Entity;

/// <summary>
/// Jogo disponível no catálogo da plataforma.
/// Cadastrar jogos e criar promoções são operações exclusivas do administrador.
/// </summary>
public class Jogo : EntityBase
{
    private readonly List<UsuarioJogo> _aquisicoes = [];

    /// <summary>
    /// Construtor sem parâmetros exigido pelo Entity Framework na leitura do banco.
    /// </summary>
    private Jogo()
    {
    }

    private Jogo(string titulo, string descricao, decimal preco, string genero, DateTime dataLancamento)
    {
        Titulo = titulo;
        Descricao = descricao;
        Preco = preco;
        Genero = genero;
        DataLancamento = dataLancamento;
    }

    public string Titulo { get; private set; } = string.Empty;

    public string Descricao { get; private set; } = string.Empty;

    /// <summary>
    /// Preço de tabela, sem desconto.
    /// </summary>
    public decimal Preco { get; private set; }

    public string Genero { get; private set; } = string.Empty;

    public DateTime DataLancamento { get; private set; }

    /// <summary>
    /// Percentual de desconto vigente (0 a 90). Zero significa sem promoção.
    /// </summary>
    public decimal PercentualDesconto { get; private set; }

    /// <summary>
    /// Usuários que adquiriram este jogo.
    /// </summary>
    public IReadOnlyCollection<UsuarioJogo> Aquisicoes => _aquisicoes.AsReadOnly();

    public static Jogo Criar(
        string titulo,
        string descricao,
        decimal preco,
        string genero,
        DateTime dataLancamento)
    {
        if (string.IsNullOrWhiteSpace(titulo))
        {
            throw new DomainException("O título do jogo é obrigatório.");
        }

        if (preco < 0)
        {
            throw new DomainException("O preço do jogo não pode ser negativo.");
        }

        return new Jogo(
            titulo.Trim(),
            descricao?.Trim() ?? string.Empty,
            preco,
            genero?.Trim() ?? string.Empty,
            dataLancamento);
    }

    public void Atualizar(string titulo, string descricao, decimal preco, string genero, DateTime dataLancamento)
    {
        if (string.IsNullOrWhiteSpace(titulo))
        {
            throw new DomainException("O título do jogo é obrigatório.");
        }

        if (preco < 0)
        {
            throw new DomainException("O preço do jogo não pode ser negativo.");
        }

        Titulo = titulo.Trim();
        Descricao = descricao?.Trim() ?? string.Empty;
        Preco = preco;
        Genero = genero?.Trim() ?? string.Empty;
        DataLancamento = dataLancamento;
    }

    /// <summary>
    /// Coloca o jogo em promoção. O limite de 90% evita que um erro de digitação
    /// zere o preço do catálogo.
    /// </summary>
    public void AplicarPromocao(decimal percentualDesconto)
    {
        if (percentualDesconto is < 0 or > 90)
        {
            throw new DomainException("O desconto deve estar entre 0% e 90%.");
        }

        PercentualDesconto = percentualDesconto;
    }

    public void EncerrarPromocao() => PercentualDesconto = 0;

    /// <summary>
    /// Preço cobrado hoje, já considerando a promoção vigente.
    ///
    /// AwayFromZero é obrigatório aqui: o padrão do Math.Round é o arredondamento
    /// bancário, que transformaria R$ 149,925 em R$ 149,92 em vez de R$ 149,93.
    /// </summary>
    public decimal PrecoAtual() =>
        Math.Round(Preco * (1 - PercentualDesconto / 100), 2, MidpointRounding.AwayFromZero);
}
