using TechChallenge.Domain.Entity;

namespace TechChallengeUnitTests;

public class UserTests
{
    [Fact]
    public void ShouldCreateUser_WhenDataIsValid()
    {
        // Arrange + Act
        var user = new User(
            "Gabriela",
            PerfilEnum.Client,
            "gabriela@email.com",
            "Password@123",
            "gabriela"
        );

        // Assert
        Assert.NotNull(user);
        Assert.Equal("Gabriela", user.Name);
        Assert.Equal("gabriela@email.com", user.Email);
        Assert.Equal("gabriela", user.Login);
        Assert.Equal(PerfilEnum.Client, user.Perfil);
    }

    [Fact]
    public void ShouldThrowException_WhenPasswordHasLessThanEightCharacters()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            new User(
                "Gabriela",
                PerfilEnum.Client,
                "gabriela@email.com",
                "Ab@123",
                "gabriela"
            );
        });
    }

    [Fact]
    public void ShouldThrowException_WhenPasswordDoesNotContainNumber()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            new User(
                "Gabriela",
                PerfilEnum.Client,
                "gabriela@email.com",
                "Password@",
                "gabriela"
            );
        });
    }

    [Fact]
    public void ShouldThrowException_WhenPasswordDoesNotContainLetter()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            new User(
                "Gabriela",
                PerfilEnum.Client,
                "gabriela@email.com",
                "12345678@",
                "gabriela"
            );
        });
    }

    [Fact]
    public void ShouldThrowException_WhenPasswordDoesNotContainSpecialCharacter()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            new User(
                "Gabriela",
                PerfilEnum.Client,
                "gabriela@email.com",
                "Password123",
                "gabriela"
            );
        });
    }

    [Fact]
    public void ShouldThrowException_WhenEmailIsInvalid()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            new User(
                "Gabriela",
                PerfilEnum.Client,
                "invalid-email",
                "Password@123",
                "gabriela"
            );
        });
    }

    [Fact]
    public void ShouldSetCreatedAt_WhenUserIsCreated()
    {
        // Arrange
        var beforeCreation = DateTime.Now;

        // Act
        var user = new User(
            "Gabriela",
            PerfilEnum.Client,
            "gabriela@email.com",
            "Password@123",
            "gabriela"
        );

        var afterCreation = DateTime.Now;

        // Assert
        Assert.InRange(
            user.CreatedAt,
            beforeCreation,
            afterCreation
        );
    }

    [Fact]
    public void ShouldThrowException_WhenUpdatingWithWeakPassword()
    {
        // Arrange
        var user = new User(
            "Gabriela",
            PerfilEnum.Client,
            "gabriela@email.com",
            "Password@123",
            "gabriela"
        );

        // Act + Assert
        Assert.Throws<ArgumentException>(() =>
        {
            user.Update(
                "Gabriela",
                PerfilEnum.Client,
                "gabriela@email.com",
                "123"
            );
        });
    }

    [Fact]
    public void ShouldThrowException_WhenUpdatingWithInvalidEmail()
    {
        // Arrange
        var user = new User(
            "Gabriela",
            PerfilEnum.Client,
            "gabriela@email.com",
            "Password@123",
            "gabriela"
        );

        // Act + Assert
        Assert.Throws<ArgumentException>(() =>
        {
            user.Update(
                "Gabriela",
                PerfilEnum.Client,
                "invalid-email",
                "Password@123"
            );
        });
    }
}