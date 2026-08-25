using Moq;
using TechChallenge.Domain.Entity;
using TechChallenge.Domain.Exceptions;
using TechChallenge.Domain.Interfaces;
using TechChallenge.Domain.Services;

namespace TechChallengeUnitTests;

public class UserGameServiceTests
{
    private readonly Mock<IUserGameRepository> _userGameRepositoryMock;

    private readonly Mock<IUserRepository> _userRepositoryMock;

    private readonly Mock<IGameRepository> _gameRepositoryMock;

    private readonly UserGameService _service;

    public UserGameServiceTests()
    {
        _userGameRepositoryMock = new Mock<IUserGameRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _gameRepositoryMock = new Mock<IGameRepository>();

        _service = new UserGameService(_userGameRepositoryMock.Object, _userRepositoryMock.Object, _gameRepositoryMock.Object);
    }

    [Fact]
    public async Task
        ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        _userRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.AddGameToLibraryAsync(1, 1));
    }

    [Fact]
    public async Task
        ShouldThrowNotFoundException_WhenGameDoesNotExist()
    {
        var user = new User(
            "User",
            "user@email.com",
            "user",
            "Password@123",
            PerfilEnum.Client
        );

        _userRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(user);

        _gameRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Game?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.AddGameToLibraryAsync(1, 1));
    }

    [Fact]
    public async Task ShouldThrowConflictException_WhenGameIsAlreadyInTheLibrary()
    {
        // Arrange
        var user = CriarUsuario();
        var game = CriarJogo();

        _userRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(user);
        _gameRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(game);

        _userGameRepositoryMock
            .Setup(x => x.GetByUserAndGameAsync(1, 1))
            .ReturnsAsync(new UserGame { UserId = 1, GameId = 1, PurchasedAt = DateTime.Now });

        // Act + Assert
        await Assert.ThrowsAsync<ConflictException>(() => _service.AddGameToLibraryAsync(1, 1));

        _userGameRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<UserGame>()),
            Times.Never
        );
    }

    [Fact]
    public async Task ShouldAddGameToLibrary_WhenUserAndGameExistAndGameIsNotOwned()
    {
        // Arrange
        var user = CriarUsuario();
        var game = CriarJogo();

        _userRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(user);
        _gameRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(game);

        _userGameRepositoryMock
            .Setup(x => x.GetByUserAndGameAsync(1, 1))
            .ReturnsAsync((UserGame?)null);

        _userGameRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<UserGame>()))
            .ReturnsAsync((UserGame userGame) => userGame);

        // Act
        var result = await _service.AddGameToLibraryAsync(1, 1);

        // Assert
        Assert.Equal(1, result.GameId);
        Assert.Equal("Minecraft", result.GameName);
        Assert.Equal(99.90m, result.Price);

        _userGameRepositoryMock.Verify(
            x => x.AddAsync(It.Is<UserGame>(userGame =>
                userGame.UserId == 1 && userGame.GameId == 1)),
            Times.Once
        );
    }

    [Fact]
    public async Task ShouldReturnUserLibrary_WhenUserExists()
    {
        // Arrange
        var user = CriarUsuario();
        var game = CriarJogo();

        _userRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(user);

        _userGameRepositoryMock
            .Setup(x => x.GetByUserIdAsync(1))
            .ReturnsAsync(new List<UserGame>
            {
                new()
                {
                    UserId = 1,
                    GameId = 1,
                    Game = game,
                    PurchasedAt = DateTime.Now
                }
            });

        // Act
        var result = await _service.GetUserLibraryAsync(1);

        // Assert
        Assert.Single(result);
        Assert.Equal("Minecraft", result[0].GameName);
        Assert.Equal(99.90m, result[0].Price);
    }

    [Fact]
    public async Task ShouldReturnEmptyLibrary_WhenUserHasNoGames()
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(CriarUsuario());

        _userGameRepositoryMock
            .Setup(x => x.GetByUserIdAsync(1))
            .ReturnsAsync(new List<UserGame>());

        // Act
        var result = await _service.GetUserLibraryAsync(1);

        // Assert
        // Biblioteca vazia é resposta válida, não "não encontrado".
        Assert.Empty(result);
    }

    [Fact]
    public async Task ShouldThrowNotFoundException_WhenGettingLibraryOfNonExistingUser()
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((User?)null);

        // Act + Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetUserLibraryAsync(999));
    }

    private static User CriarUsuario() =>
        new("User", "user@email.com", "user", "Password@123", PerfilEnum.Client);

    private static Game CriarJogo() =>
        new()
        {
            Id = 1,
            Name = "Minecraft",
            Description = "Sandbox",
            Price = 99.90m,
            IsActive = true
        };
}