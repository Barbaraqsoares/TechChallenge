using TechChallenge.Domain.Entity;
using TechChallenge.Domain.Exceptions;

namespace TechChallengeUnitTests.Entity;

/// <summary>
/// Testes das regras de catálogo e promoções — funcionalidades exclusivas do
/// administrador segundo o desafio.
/// </summary>
public class JogoTests
{
    private static Jogo CriarJogo(decimal preco = 100m) =>
        Jogo.Criar("The Legend of FIAP", "Aventura pela tecnologia", preco, "Aventura", new DateTime(2026, 1, 15));

    [Fact]
    public void Criar_ComDadosValidos_DeveNascerSemPromocao()
    {
        // Act
        var jogo = CriarJogo(preco: 199.90m);

        // Assert
        Assert.Equal(0, jogo.PercentualDesconto);
        Assert.Equal(199.90m, jogo.PrecoAtual());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_SemTitulo_DeveLancarExcecao(string tituloVazio)
    {
        // Act
        var excecao = Assert.Throws<DomainException>(
            () => Jogo.Criar(tituloVazio, "descrição", 100m, "Aventura", DateTime.Today));

        // Assert
        Assert.Equal("O título do jogo é obrigatório.", excecao.Message);
    }

    [Fact]
    public void Criar_ComPrecoNegativo_DeveLancarExcecao()
    {
        // Act
        var excecao = Assert.Throws<DomainException>(
            () => Jogo.Criar("Jogo", "descrição", -1m, "Aventura", DateTime.Today));

        // Assert
        Assert.Equal("O preço do jogo não pode ser negativo.", excecao.Message);
    }

    [Theory]
    [InlineData(0, 200)]
    [InlineData(25, 150)]
    [InlineData(50, 100)]
    [InlineData(90, 20)]
    public void AplicarPromocao_DeveCalcularOPrecoComDesconto(decimal desconto, decimal precoEsperado)
    {
        // Arrange
        var jogo = CriarJogo(preco: 200m);

        // Act
        jogo.AplicarPromocao(desconto);

        // Assert
        Assert.Equal(precoEsperado, jogo.PrecoAtual());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(91)]
    [InlineData(150)]
    public void AplicarPromocao_ForaDoLimite_DeveLancarExcecao(decimal descontoInvalido)
    {
        // Arrange
        var jogo = CriarJogo();

        // Act
        var excecao = Assert.Throws<DomainException>(() => jogo.AplicarPromocao(descontoInvalido));

        // Assert
        Assert.Equal("O desconto deve estar entre 0% e 90%.", excecao.Message);
    }

    [Fact]
    public void EncerrarPromocao_DeveVoltarAoPrecoDeTabela()
    {
        // Arrange
        var jogo = CriarJogo(preco: 200m);
        jogo.AplicarPromocao(30);

        // Act
        jogo.EncerrarPromocao();

        // Assert
        Assert.Equal(200m, jogo.PrecoAtual());
        Assert.Equal(0, jogo.PercentualDesconto);
    }

    [Fact]
    public void PrecoAtual_DeveArredondarParaDuasCasas()
    {
        // Arrange — 199,90 com 25% resulta em 149,925.
        var jogo = CriarJogo(preco: 199.90m);

        // Act
        jogo.AplicarPromocao(25);

        // Assert
        Assert.Equal(149.93m, jogo.PrecoAtual());
    }
}
