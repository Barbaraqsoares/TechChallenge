using TechChallenge.Domain.Exceptions;
using TechChallenge.Domain.ValueObject;

namespace TechChallengeUnitTests.ValueObject;

/// <summary>
/// Testes da política de senha segura exigida pelo desafio:
/// mínimo de 8 caracteres, com letras, números e caracteres especiais.
///
/// Este é o módulo desenvolvido com TDD (Red-Green-Refactor): cada regra nasceu
/// de um teste que primeiro falhou, e só depois foi implementada na classe Senha.
/// Os testes seguem o padrão AAA — Arrange, Act, Assert.
/// </summary>
public class SenhaTests
{
    [Theory]
    [InlineData("Fiap@2026")]
    [InlineData("Senha#123")]
    [InlineData("A1b2C3d4!")]
    public void Criar_ComSenhaValida_DeveGerarHash(string senhaValida)
    {
        // Arrange & Act
        var senha = Senha.Criar(senhaValida);

        // Assert
        Assert.NotNull(senha.Hash);
        Assert.NotEmpty(senha.Hash);
    }

    [Fact]
    public void Criar_ComMenosDeOitoCaracteres_DeveLancarExcecao()
    {
        // Arrange
        var senhaCurta = "Ab@123";

        // Act
        var excecao = Assert.Throws<DomainException>(() => Senha.Criar(senhaCurta));

        // Assert
        Assert.Equal("A senha deve ter no mínimo 8 caracteres.", excecao.Message);
    }

    [Fact]
    public void Criar_SemLetra_DeveLancarExcecao()
    {
        // Arrange
        var senhaSemLetra = "12345678@";

        // Act
        var excecao = Assert.Throws<DomainException>(() => Senha.Criar(senhaSemLetra));

        // Assert
        Assert.Equal("A senha deve conter ao menos uma letra.", excecao.Message);
    }

    [Fact]
    public void Criar_SemNumero_DeveLancarExcecao()
    {
        // Arrange
        var senhaSemNumero = "SenhaForte@";

        // Act
        var excecao = Assert.Throws<DomainException>(() => Senha.Criar(senhaSemNumero));

        // Assert
        Assert.Equal("A senha deve conter ao menos um número.", excecao.Message);
    }

    [Fact]
    public void Criar_SemCaractereEspecial_DeveLancarExcecao()
    {
        // Arrange
        var senhaSemEspecial = "Senha12345";

        // Act
        var excecao = Assert.Throws<DomainException>(() => Senha.Criar(senhaSemEspecial));

        // Assert
        Assert.Equal("A senha deve conter ao menos um caractere especial.", excecao.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Criar_ComSenhaVazia_DeveLancarExcecao(string? senhaVazia)
    {
        // Act
        var excecao = Assert.Throws<DomainException>(() => Senha.Criar(senhaVazia!));

        // Assert
        Assert.Equal("A senha é obrigatória.", excecao.Message);
    }

    [Fact]
    public void Hash_NuncaDeveConterASenhaEmTextoPuro()
    {
        // Arrange
        var senhaEmTextoPuro = "Fiap@2026";

        // Act
        var senha = Senha.Criar(senhaEmTextoPuro);

        // Assert
        Assert.DoesNotContain(senhaEmTextoPuro, senha.Hash);
    }

    [Fact]
    public void Criar_ComAMesmaSenhaDuasVezes_DeveGerarHashesDiferentes()
    {
        // Arrange & Act — cada senha recebe um salt aleatório próprio.
        var primeira = Senha.Criar("Fiap@2026");
        var segunda = Senha.Criar("Fiap@2026");

        // Assert
        Assert.NotEqual(primeira.Hash, segunda.Hash);
    }

    [Fact]
    public void Conferir_ComASenhaCorreta_DeveRetornarVerdadeiro()
    {
        // Arrange
        var senha = Senha.Criar("Fiap@2026");

        // Act
        var confere = senha.Conferir("Fiap@2026");

        // Assert
        Assert.True(confere);
    }

    [Theory]
    [InlineData("SenhaErrada@1")]
    [InlineData("fiap@2026")]
    [InlineData("")]
    public void Conferir_ComSenhaIncorreta_DeveRetornarFalso(string senhaIncorreta)
    {
        // Arrange
        var senha = Senha.Criar("Fiap@2026");

        // Act
        var confere = senha.Conferir(senhaIncorreta);

        // Assert
        Assert.False(confere);
    }
}
