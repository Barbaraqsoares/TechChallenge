using TechChallenge.Domain.Entity;

namespace TechChallenge.Domain.Output;

/// <summary>
/// Espelho do jogo para transitar na API.
/// </summary>
public class JogoOutput
{
    public int Id { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    /// <summary>Preço de tabela, sem desconto.</summary>
    public decimal Preco { get; set; }

    /// <summary>Preço cobrado hoje, já com a promoção aplicada.</summary>
    public decimal PrecoAtual { get; set; }

    public decimal PercentualDesconto { get; set; }

    public string Genero { get; set; } = string.Empty;

    public DateTime DataLancamento { get; set; }

    public static JogoOutput De(Jogo jogo) => new()
    {
        Id = jogo.Id,
        Titulo = jogo.Titulo,
        Descricao = jogo.Descricao,
        Preco = jogo.Preco,
        PrecoAtual = jogo.PrecoAtual(),
        PercentualDesconto = jogo.PercentualDesconto,
        Genero = jogo.Genero,
        DataLancamento = jogo.DataLancamento
    };
}

/// <summary>
/// Item da biblioteca de jogos do usuário.
/// </summary>
public class JogoAdquiridoOutput
{
    public int JogoId { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string Genero { get; set; } = string.Empty;

    /// <summary>Quanto foi pago na época da compra.</summary>
    public decimal PrecoPago { get; set; }

    public DateTime DataAquisicao { get; set; }

    public static JogoAdquiridoOutput De(UsuarioJogo aquisicao) => new()
    {
        JogoId = aquisicao.JogoId,
        Titulo = aquisicao.Jogo?.Titulo ?? string.Empty,
        Genero = aquisicao.Jogo?.Genero ?? string.Empty,
        PrecoPago = aquisicao.PrecoPago,
        DataAquisicao = aquisicao.DataAquisicao
    };
}
