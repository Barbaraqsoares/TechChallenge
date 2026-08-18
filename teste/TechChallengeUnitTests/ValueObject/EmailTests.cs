using TechChallenge.Domain.Exceptions;
using TechChallenge.Domain.ValueObject;

namespace TechChallengeUnitTests.ValueObject;

/// <summary>
/// Testes da validação de formato de e-mail exigida pelo desafio.
/// </summary>
public class EmailTests
{
    [Theory]
    [InlineData("maria@fiap.com.br")]
    [InlineData("joao.silva@alura.com")]
    [InlineData("aluno+tag@pm3.com.br")]
    public void Criar_ComEmailValido_DeveCriarObjeto(string emailValido)
    {
        // Act
        var email = Email.Criar(emailValido);

        // Assert
        Assert.Equal(emailValido.ToLowerInvariant(), email.Endereco);
    }

    [Theory]
    [InlineData("maria")]
    [InlineData("maria@")]
    [InlineData("@fiap.com")]
    [InlineData("maria@fiap")]
    [InlineData("maria fiap@teste.com")]
    public void Criar_ComEmailInvalido_DeveLancarExcecao(string emailInvalido)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => Email.Criar(emailInvalido));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_ComEmailVazio_DeveLancarExcecao(string emailVazio)
    {
        // Act
        var excecao = Assert.Throws<DomainException>(() => Email.Criar(emailVazio));

        // Assert
        Assert.Equal("O e-mail é obrigatório.", excecao.Message);
    }

    [Fact]
    public void Criar_DeveNormalizarParaMinusculasSemEspacos()
    {
        // Arrange
        var digitadoPeloUsuario = "  MARIA@FIAP.COM.BR  ";

        // Act
        var email = Email.Criar(digitadoPeloUsuario);

        // Assert
        Assert.Equal("maria@fiap.com.br", email.Endereco);
    }

    [Fact]
    public void Emails_ComMesmoEndereco_DevemSerIguais()
    {
        // Arrange — Objeto de Valor é comparado pelo conteúdo, não por identidade.
        var primeiro = Email.Criar("maria@fiap.com.br");
        var segundo = Email.Criar("MARIA@FIAP.COM.BR");

        // Assert
        Assert.Equal(primeiro, segundo);
    }
}
