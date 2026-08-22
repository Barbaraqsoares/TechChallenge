using TechChallenge.Domain.Validators;

namespace TechChallengeUnitTests;

public class EmailValidatorTests
{
    [Fact]
    public void Email_VazioOuNulo_DeveSerInvalido()
    {
        // Arrange
        string email = null;

        // Act
        var resultado = EmailValidator.EhValido(email);

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public void Email_SemArroba_DeveSerInvalido()
    {
        // Arrange
        string email = "email.com";

        // Act
        var resultado = EmailValidator.EhValido(email);

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public void Email_SemNadaDepoisDoArroba_DeveSerInvalido()
    {
        // Arrange
        string email = "email@";

        // Act
        var resultado = EmailValidator.EhValido(email);

        // Assert
        Assert.False(resultado);
    }
    [Fact]
    public void Email_SemPontoNoDominio_DeveSerinvalido()
    {
        // Arrange
        string email = "email@dominio";

        // Act
        var resultado = EmailValidator.EhValido(email);

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public void Email_ComTodosOsRequisitos_DeveSerValido()
    {
        // Arrange
        string email = "email@dominio.com";

        // Act
        var resultado = EmailValidator.EhValido(email);

        // Assert
        Assert.True(resultado);
    }
}