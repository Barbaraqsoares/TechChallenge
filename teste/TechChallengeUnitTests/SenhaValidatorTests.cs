using TechChallenge.Domain.Validators;

namespace TechChallengeUnitTests;

public class SenhaValidatorTests
{
    [Fact]
    public void Senha_ComMenosDeOitoCaracteres_DeveSerInvalida()
    {
        // Arrange
        var senha = "Abc1!";

        // Act
        var resultado = SenhaValidator.EhValida(senha);

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public void Senha_SemNumero_DeveSerInvalida()
    {
        // Arrange
        var senha = "AbcdefghFFF";
        
        // Act
        var resultado = SenhaValidator.EhValida(senha);

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public void Senha_SemLetra_DeveSerInvalida()
    {
        // Arrange
        var senha = "12345678!";

        // Act
        var resultado = SenhaValidator.EhValida(senha);

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public void Senha_SemCaractereEspecial_DeveserInvalida()
    {
        // Arrange
        var senha = "AbcdefghiJ12";

        // Act
        var resultado = SenhaValidator.EhValida(senha);

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public void Senha_ComTodosOsRequisitos_DeveSerValida()
    {
        // Arrange
        var senha = "Apocalipse123!";

        // Act
        var resultado = SenhaValidator.EhValida(senha);

        // Assert
        Assert.True(resultado);
    }
}