namespace TechChallenge.Domain.Input;

/// <summary>
/// Dados para cadastro de um jogo no catálogo.
/// </summary>
public class JogoInput
{
    /// <example>The Legend of FIAP</example>
    public required string Titulo { get; set; }

    /// <example>Uma aventura pelos caminhos da tecnologia.</example>
    public string Descricao { get; set; } = string.Empty;

    /// <example>199.90</example>
    public required decimal Preco { get; set; }

    /// <example>Aventura</example>
    public string Genero { get; set; } = string.Empty;

    /// <example>2026-01-15</example>
    public required DateTime DataLancamento { get; set; }
}

/// <summary>
/// Dados para atualização de um jogo já cadastrado.
/// </summary>
public class JogoUpdateInput : JogoInput
{
    public required int Id { get; set; }
}

/// <summary>
/// Percentual de desconto de uma promoção.
/// </summary>
public class PromocaoInput
{
    /// <summary>Entre 0 e 90. Use 0 para encerrar a promoção.</summary>
    /// <example>25</example>
    public required decimal PercentualDesconto { get; set; }
}
