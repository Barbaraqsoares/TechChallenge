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
}