using Moq;
using TechChallenge.Domain.Entity;
using TechChallenge.Domain.Exceptions;
using TechChallenge.Domain.Interfaces;
using TechChallenge.Domain.Models.Games;
using TechChallenge.Domain.Services;

namespace TechChallengeUnitTests;

public class GameServiceTests
{
    [Fact]
    public async Task ShouldCreateGame_WhenDataIsValid()
    {
        // Arrange
        var repositoryMock = new Mock<IGameRepository>();

        var game = new CreateGameRequest
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

        var game = new CreateGameRequest
        {
            Name = "",
            Description = "Test game",
            Price = 50,
            IsMultiplayer = false
        };

        // Act + Assert
        await Assert.ThrowsAsync<DomainException>(
            async () => await service.CreateAsync(game)
        );
    }

    [Fact]
    public async Task ShouldThrowException_WhenGamePriceIsNegative()
    {
        // Arrange
        var repositoryMock = new Mock<IGameRepository>();

        var service = new GameService(repositoryMock.Object);

        var game = new CreateGameRequest
        {
            Name = "Test Game",
            Description = "Test description",
            Price = -10,
            IsMultiplayer = false
        };

        // Act + Assert
        await Assert.ThrowsAsync<DomainException>(
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

        var game = new CreateGameRequest
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

        var game = new CreateGameRequest
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

        // UpdatedAt é anulável em EntityBase, então precisa ser desembrulhado antes
        // de comparar — o próprio Assert.NotNull já protege contra um nulo silencioso.
        Assert.NotNull(result.UpdatedAt);

        Assert.InRange(
            result.UpdatedAt.Value,
            beforeCreation,
            afterCreation
        );
    }

    [Fact]
    public async Task ShouldThrowNotFoundException_WhenGameDoesNotExist()
    {
        // Arrange
        var repositoryMock = new Mock<IGameRepository>();

        repositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(999))
            .ReturnsAsync((Game?)null);

        var service = new GameService(repositoryMock.Object);

        // Act + Assert
        await Assert.ThrowsAsync<NotFoundException>(
            async () => await service.GetByIdAsync(999)
        );
    }

    [Fact]
    public async Task ShouldUpdateGame_WhenGameExists()
    {
        // Arrange
        var repositoryMock = new Mock<IGameRepository>();

        var createdAt = DateTime.Now.AddDays(-1);

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

        var updatedGame = new UpdateGameRequest
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
    public async Task ShouldThrowNotFoundException_WhenUpdatingNonExistingGame()
    {
        // Arrange
        var repositoryMock = new Mock<IGameRepository>();

        repositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(999))
            .ReturnsAsync((Game?)null);

        var service = new GameService(repositoryMock.Object);

        var game = new UpdateGameRequest
        {
            Name = "Test Game",
            Price = 100
        };

        // Act + Assert
        await Assert.ThrowsAsync<NotFoundException>(
            async () => await service.UpdateAsync(999, game)
        );

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
        await service.DeleteAsync(1);

        // Assert
        repositoryMock.Verify(
            repository =>
                repository.DeleteAsync(game),
            Times.Once
        );
    }

    [Fact]
    public async Task ShouldThrowNotFoundException_WhenDeletingNonExistingGame()
    {
        // Arrange
        var repositoryMock = new Mock<IGameRepository>();

        repositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(999))
            .ReturnsAsync((Game?)null);

        var service = new GameService(repositoryMock.Object);

        // Act + Assert
        await Assert.ThrowsAsync<NotFoundException>(
            async () => await service.DeleteAsync(999)
        );

        repositoryMock.Verify(
            repository =>
                repository.DeleteAsync(
                    It.IsAny<Game>()
                ),
            Times.Never
        );
    }

    [Fact]
    public async Task ShouldReturnGame_WhenGameExists()
    {
        // Arrange
        var repositoryMock = new Mock<IGameRepository>();

        var game = new Game
        {
            Id = 1,
            Name = "Minecraft",
            Description = "Sandbox",
            Price = 99.90m,
            IsActive = true
        };

        repositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(1))
            .ReturnsAsync(game);

        var service = new GameService(repositoryMock.Object);

        // Act
        var result = await service.GetByIdAsync(1);

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal("Minecraft", result.Name);
    }

    [Fact]
    public async Task ShouldReturnAllGames()
    {
        // Arrange
        var repositoryMock = new Mock<IGameRepository>();

        var games = new List<Game>
        {
            new() { Id = 1, Name = "Minecraft", Price = 99.90m },
            new() { Id = 2, Name = "Stardew Valley", Price = 34.90m }
        };

        repositoryMock
            .Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(games);

        var service = new GameService(repositoryMock.Object);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, game => game.Name == "Stardew Valley");
    }

    [Fact]
    public async Task ShouldReturnEmptyList_WhenThereAreNoGames()
    {
        // Arrange
        var repositoryMock = new Mock<IGameRepository>();

        repositoryMock
            .Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(new List<Game>());

        var service = new GameService(repositoryMock.Object);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        // Lista vazia é resposta válida — não deve virar "não encontrado".
        Assert.Empty(result);
    }
}