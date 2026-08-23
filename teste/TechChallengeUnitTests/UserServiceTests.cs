using Moq;
using TechChallenge.Domain.Entity;
using TechChallenge.Domain.Interfaces;
using TechChallenge.Domain.Models.User;
using TechChallenge.Domain.Services;

namespace TechChallengeUnitTests;

public class UserServiceTests
{
    [Fact]
    public async Task ShouldThrowException_WhenLoginAlreadyExists()
    {
        // Arrange
        var repositoryMock = new Mock<IUserRepository>();

        var existingUser = new User(
            "Existing User",
            PerfilEnum.Client,
            "existing@email.com",
            "Password@123",
            "gabriela"
        );

        repositoryMock
            .Setup(repository =>
                repository.GetByLoginAsync("gabriela"))
            .ReturnsAsync(existingUser);

        var service = new UserService(repositoryMock.Object);

        var request = new RegisterUserRequest
        {
            Name = "Gabriela",
            Email = "new@email.com",
            Login = "gabriela",
            Password = "Password@123"
        };

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await service.CreateAsync(request)
        );
    }

    [Fact]
    public async Task ShouldThrowException_WhenEmailAlreadyExists()
    {
        // Arrange
        var repositoryMock = new Mock<IUserRepository>();

        var existingUser = new User(
            "Existing User",
            PerfilEnum.Client,
            "existing@email.com",
            "Password@123",
            "existinguser"
        );

        repositoryMock
            .Setup(repository =>
                repository.GetByLoginAsync("gabriela"))
            .ReturnsAsync((User?)null);

        repositoryMock
            .Setup(repository =>
                repository.GetByEmailAsync("existing@email.com"))
            .ReturnsAsync(existingUser);

        var service = new UserService(repositoryMock.Object);

        var request = new RegisterUserRequest
        {
            Name = "Gabriela",
            Email = "existing@email.com",
            Login = "gabriela",
            Password = "Password@123"
        };

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await service.CreateAsync(request)
        );
    }

    [Fact]
    public async Task ShouldCreateUser_WhenLoginAndEmailDoNotExist()
    {
        // Arrange
        var repositoryMock = new Mock<IUserRepository>();

        repositoryMock
            .Setup(repository =>
                repository.GetByLoginAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        repositoryMock
            .Setup(repository =>
                repository.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        repositoryMock
            .Setup(repository =>
                repository.AddAsync(It.IsAny<User>()))
            .ReturnsAsync((User user) =>
            {
                user.Id = 1;
                return user;
            });

        var service = new UserService(repositoryMock.Object);

        var request = new RegisterUserRequest
        {
            Name = "Gabriela",
            Email = "gabriela@email.com",
            Login = "gabriela",
            Password = "Password@123"
        };

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Gabriela", result.Name);
        Assert.Equal("gabriela@email.com", result.Email);
        Assert.Equal("gabriela", result.Login);

        repositoryMock.Verify(
            repository =>
                repository.AddAsync(It.IsAny<User>()),
            Times.Once
        );
    }
}
