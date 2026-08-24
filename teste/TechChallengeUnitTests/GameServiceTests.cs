using Moq;
using TechChallenge.Domain.Entity;
using TechChallenge.Domain.Interfaces;
using TechChallenge.Domain.Services;

namespace TechChallengeUnitTests;

public class GameServiceTests
{
    [Fact]
    public async Task ShouldCreateGame_WhenDataIsValid()
    {
        // Arrange
        var repositoryMock = new Mock<IGameRepository>();

        var game = new Game
        {
            Name = "Minecraft",
            Description = "Building and exploration game",
            Price = 99.90m,
            IsMultiplayer = true
        };

        repositoryMock
            .Setup(repository =>
                repository.AddAsync(It.IsAny<Game>()))
            .ReturnsAsync((Game game) => game);

        var service = new GameService(repositoryMock.Object);

        // Act
        var result = await service.CreateAsync(game);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Minecraft", result.Name);
        Assert.Equal(99.90m, result.Price);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task ShouldThrowException_WhenGameNameIsEmpty()
    {
        // Arrange
        var repositoryMock = new Mock<IGameRepository>();

        var service = new GameService(repositoryMock.Object);

        var game = new Game
        {
            Name = "",
            Description = "Test game",
            Price = 50,
            IsMultiplayer = false
        };

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await service.CreateAsync(game)
        );
    }

    [Fact]
    public async Task ShouldThrowException_WhenGamePriceIsNegative()
    {
        // Arrange
        var repositoryMock = new Mock<IGameRepository>();

        var service = new GameService(repositoryMock.Object);

        var game = new Game
        {
            Name = "Test Game",
            Description = "Test description",
            Price = -10,
            IsMultiplayer = false
        };

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await service.CreateAsync(game)
        );
    }

    [Fact]
    public async Task ShouldCreateGameAsActive()
    {
        // Arrange
        var repositoryMock = new Mock<IGameRepository>();

        repositoryMock
            .Setup(repository =>
                repository.AddAsync(It.IsAny<Game>()))
            .ReturnsAsync((Game game) => game);

        var service = new GameService(repositoryMock.Object);

        var game = new Game
        {
            Name = "Minecraft",
            Price = 99.90m
        };

        // Act
        var result = await service.CreateAsync(game);

        // Assert
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task ShouldSetCreatedAtAndUpdatedAt_WhenGameIsCreated()
    {
        // Arrange
        var repositoryMock = new Mock<IGameRepository>();

        repositoryMock
            .Setup(repository =>
                repository.AddAsync(It.IsAny<Game>()))
            .ReturnsAsync((Game game) => game);

        var service = new GameService(repositoryMock.Object);

        var game = new Game
        {
            Name = "Minecraft",
            Price = 99.90m
        };

        var beforeCreation = DateTime.Now;

        // Act
        var result = await service.CreateAsync(game);

        var afterCreation = DateTime.Now;

        // Assert
        Assert.InRange(
            result.CreatedAt,
            beforeCreation,
            afterCreation
        );

        Assert.InRange(
            result.UpdatedAt,
            beforeCreation,
            afterCreation
        );
    }

    [Fact]
    public async Task ShouldReturnNull_WhenGameDoesNotExist()
    {
        // Arrange
        var repositoryMock = new Mock<IGameRepository>();

        repositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(999))
            .ReturnsAsync((Game?)null);

        var service = new GameService(repositoryMock.Object);

        // Act
        var result = await service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ShouldUpdateGame_WhenGameExists()
    {
        // Arrange
        var repositoryMock = new Mock<IGameRepository>();

        var createdAt = DateTime.UtcNow.AddDays(-1);

        var existingGame = new Game
        {
            Id = 1,
            Name = "Minecraft",
            Description = "Old description",
            Price = 99.90m,
            IsActive = true,
            IsMultiplayer = false,
            CreatedAt = createdAt,
            UpdatedAt = createdAt
        };

        repositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(1))
            .ReturnsAsync(existingGame);

        var service = new GameService(repositoryMock.Object);

        var updatedGame = new Game
        {
            Name = "Minecraft Deluxe",
            Description = "Updated description",
            Price = 129.90m,
            IsActive = true,
            IsMultiplayer = true
        };

        // Act
        var result = await service.UpdateAsync(
            1,
            updatedGame
        );

        // Assert
        Assert.NotNull(result);

        Assert.Equal(
            "Minecraft Deluxe",
            result.Name
        );

        Assert.Equal(
            "Updated description",
            result.Description
        );

        Assert.Equal(
            129.90m,
            result.Price
        );

        Assert.True(result.IsMultiplayer);

        Assert.Equal(
            createdAt,
            result.CreatedAt
        );

        repositoryMock.Verify(
            repository =>
                repository.UpdateAsync(
                    It.IsAny<Game>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task ShouldReturnNull_WhenUpdatingNonExistingGame()
    {
        // Arrange
        var repositoryMock = new Mock<IGameRepository>();

        repositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(999))
            .ReturnsAsync((Game?)null);

        var service = new GameService(repositoryMock.Object);

        var game = new Game
        {
            Name = "Test Game",
            Price = 100
        };

        // Act
        var result = await service.UpdateAsync(
            999,
            game
        );

        // Assert
        Assert.Null(result);

        repositoryMock.Verify(
            repository =>
                repository.UpdateAsync(
                    It.IsAny<Game>()
                ),
            Times.Never
        );
    }

    [Fact]
    public async Task ShouldDeleteGame_WhenGameExists()
    {
        // Arrange
        var repositoryMock = new Mock<IGameRepository>();

        var game = new Game
        {
            Id = 1,
            Name = "Minecraft",
            Price = 99.90m
        };

        repositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(1))
            .ReturnsAsync(game);

        var service = new GameService(repositoryMock.Object);

        // Act
        var result = await service.DeleteAsync(1);

        // Assert
        Assert.True(result);

        repositoryMock.Verify(
            repository =>
                repository.DeleteAsync(game),
            Times.Once
        );
    }

    [Fact]
    public async Task ShouldReturnFalse_WhenDeletingNonExistingGame()
    {
        // Arrange
        var repositoryMock = new Mock<IGameRepository>();

        repositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(999))
            .ReturnsAsync((Game?)null);

        var service = new GameService(repositoryMock.Object);

        // Act
        var result = await service.DeleteAsync(999);

        // Assert
        Assert.False(result);

        repositoryMock.Verify(
            repository =>
                repository.DeleteAsync(
                    It.IsAny<Game>()
                ),
            Times.Never
        );
    }
}