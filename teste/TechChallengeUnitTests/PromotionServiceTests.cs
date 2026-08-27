using Moq;
using TechChallenge.Domain.Entity;
using TechChallenge.Domain.Exceptions;
using TechChallenge.Domain.Interfaces;
using TechChallenge.Domain.Models.Promotion;
using TechChallenge.Domain.Services;

namespace TechChallengeUnitTests;

public class PromotionServiceTests
{
    private readonly Mock<IPromotionRepository> _promotionRepositoryMock;
    private readonly Mock<IGameRepository> _gameRepositoryMock;
    private readonly PromotionService _promotionService;

    public PromotionServiceTests()
    {
        _promotionRepositoryMock =
            new Mock<IPromotionRepository>();

        _gameRepositoryMock =
            new Mock<IGameRepository>();

        _promotionService =
            new PromotionService(
                _promotionRepositoryMock.Object,
                _gameRepositoryMock.Object
            );
    }

    [Fact]
    public async Task ShouldCreatePromotion_WhenDataIsValid()
    {
        // Arrange
        var request = new CreatePromotionRequest
        {
            Name = "Black Friday",
            Discount = 20,
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(10),
            GameIds = new List<int> { 1, 2 }
        };

        var games = new List<Game>
        {
            new Game
            {
                Id = 1,
                Name = "Game 1",
                Price = 100,
                IsActive = true
            },
            new Game
            {
                Id = 2,
                Name = "Game 2",
                Price = 200,
                IsActive = true
            }
        };

        _gameRepositoryMock
            .Setup(repository =>
                repository.GetByIdsAsync(request.GameIds))
            .ReturnsAsync(games);

        _promotionRepositoryMock
            .Setup(repository =>
                repository.AddAsync(
                    It.IsAny<Promotion>()))
            .ReturnsAsync(
                (Promotion promotion) =>
                {
                    promotion.Id = 1;
                    return promotion;
                });

        // Act
        var result =
            await _promotionService.CreateAsync(
                request,
                adminUserId: 1
            );

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Black Friday", result.Name);
        Assert.Equal(20, result.Discount);
        Assert.True(result.IsActive);
        Assert.Equal(1, result.CreatedByUserId);
        Assert.Equal(2, result.GameIds.Count);

        _promotionRepositoryMock.Verify(
            repository =>
                repository.AddAsync(
                    It.IsAny<Promotion>()),
            Times.Once
        );
    }

    [Fact]
    public async Task ShouldRejectPromotion_WhenNameIsEmpty()
    {
        // Arrange
        var request = new CreatePromotionRequest
        {
            Name = "",
            Discount = 20,
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(10),
            GameIds = new List<int> { 1 }
        };

        // Act
        var exception =
            await Assert.ThrowsAsync<DomainException>(
                () =>
                    _promotionService.CreateAsync(
                        request,
                        adminUserId: 1
                    )
            );

        // Assert
        Assert.Equal(
            "O nome da promoção é obrigatório.",
            exception.Message
        );

        _promotionRepositoryMock.Verify(
            repository =>
                repository.AddAsync(
                    It.IsAny<Promotion>()),
            Times.Never
        );
    }

    [Fact]
    public async Task ShouldRejectPromotion_WhenDiscountIsZero()
    {
        // Arrange
        var request = new CreatePromotionRequest
        {
            Name = "Promotion",
            Discount = 0,
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(10),
            GameIds = new List<int> { 1 }
        };

        // Act
        var exception =
            await Assert.ThrowsAsync<DomainException>(
                () =>
                    _promotionService.CreateAsync(
                        request,
                        adminUserId: 1
                    )
            );

        // Assert
        Assert.Equal(
            "Desconto precisa ser maior que 0 e até 100.",
            exception.Message
        );
    }

    [Fact]
    public async Task ShouldRejectPromotion_WhenDiscountIsGreaterThan100()
    {
        // Arrange
        var request = new CreatePromotionRequest
        {
            Name = "Promotion",
            Discount = 101,
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(10),
            GameIds = new List<int> { 1 }
        };

        // Act
        var exception =
            await Assert.ThrowsAsync<DomainException>(
                () =>
                    _promotionService.CreateAsync(
                        request,
                        adminUserId: 1
                    )
            );

        // Assert
        Assert.Equal(
            "Desconto precisa ser maior que 0 e até 100.",
            exception.Message
        );
    }

    [Fact]
    public async Task ShouldRejectPromotion_WhenStartDateIsAfterEndDate()
    {
        // Arrange
        var request = new CreatePromotionRequest
        {
            Name = "Promotion",
            Discount = 20,
            StartDate = DateTime.Now.AddDays(10),
            EndDate = DateTime.Now.AddDays(1),
            GameIds = new List<int> { 1 }
        };

        // Act
        var exception =
            await Assert.ThrowsAsync<DomainException>(
                () =>
                    _promotionService.CreateAsync(
                        request,
                        adminUserId: 1
                    )
            );

        // Assert
        Assert.Equal(
            "A data de inicio precisa ser menor que a data fim.",
            exception.Message
        );
    }

    [Fact]
    public async Task ShouldRejectPromotion_WhenStartDateEqualsEndDate()
    {
        // Arrange
        var date = DateTime.Now.AddDays(1);

        var request = new CreatePromotionRequest
        {
            Name = "Promotion",
            Discount = 20,
            StartDate = date,
            EndDate = date,
            GameIds = new List<int> { 1 }
        };

        // Act
        var exception =
            await Assert.ThrowsAsync<DomainException>(
                () =>
                    _promotionService.CreateAsync(
                        request,
                        adminUserId: 1
                    )
            );

        // Assert
        Assert.Equal(
            "A data de inicio precisa ser menor que a data fim.",
            exception.Message
        );
    }

    [Fact]
    public async Task ShouldRejectPromotion_WhenNoGamesAreSelected()
    {
        // Arrange
        var request = new CreatePromotionRequest
        {
            Name = "Promotion",
            Discount = 20,
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(10),
            GameIds = new List<int>()
        };

        // Act
        var exception =
            await Assert.ThrowsAsync<DomainException>(
                () =>
                    _promotionService.CreateAsync(
                        request,
                        adminUserId: 1
                    )
            );

        // Assert
        Assert.Equal(
            "Ao menos 1 game precisa ser selecionado.",
            exception.Message
        );
    }

    [Fact]
    public async Task ShouldRejectPromotion_WhenGameDoesNotExist()
    {
        // Arrange
        var request = new CreatePromotionRequest
        {
            Name = "Promotion",
            Discount = 20,
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(10),
            GameIds = new List<int> { 1, 2 }
        };

        var games = new List<Game>
        {
            new Game
            {
                Id = 1,
                Name = "Game 1",
                Price = 100,
                IsActive = true
            }
        };

        _gameRepositoryMock
            .Setup(repository =>
                repository.GetByIdsAsync(
                    request.GameIds))
            .ReturnsAsync(games);

        // Act
        var exception =
            await Assert.ThrowsAsync<DomainException>(
                () =>
                    _promotionService.CreateAsync(
                        request,
                        adminUserId: 1
                    )
            );

        // Assert
        Assert.Equal(
            "Um ou mais games não foram encontrados.",
            exception.Message
        );

        _promotionRepositoryMock.Verify(
            repository =>
                repository.AddAsync(
                    It.IsAny<Promotion>()),
            Times.Never
        );
    }

    [Fact]
    public async Task ShouldReturnAllPromotions()
    {
        // Arrange
        var promotions = new List<Promotion>
        {
            new Promotion
            {
                Id = 1,
                Name = "Promotion 1",
                Discount = 10,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(5),
                IsActive = true,
                CreatedByUserId = 1,
                CreatedAt = DateTime.Now,
                Games = new List<Game>
                {
                    new Game
                    {
                        Id = 1,
                        Name = "Game 1",
                        Price = 100
                    }
                }
            },
            new Promotion
            {
                Id = 2,
                Name = "Promotion 2",
                Discount = 20,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(10),
                IsActive = true,
                CreatedByUserId = 1,
                CreatedAt = DateTime.Now,
                Games = new List<Game>
                {
                    new Game
                    {
                        Id = 2,
                        Name = "Game 2",
                        Price = 200
                    }
                }
            }
        };

        _promotionRepositoryMock
            .Setup(repository =>
                repository.GetAllAsync())
            .ReturnsAsync(promotions);

        // Act
        var result =
            await _promotionService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task ShouldReturnPromotion_WhenPromotionExists()
    {
        // Arrange
        var promotion = new Promotion
        {
            Id = 1,
            Name = "Black Friday",
            Discount = 25,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(10),
            IsActive = true,
            CreatedByUserId = 1,
            CreatedAt = DateTime.Now,
            Games = new List<Game>
            {
                new Game
                {
                    Id = 1,
                    Name = "Game 1",
                    Price = 100
                }
            }
        };

        _promotionRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(1))
            .ReturnsAsync(promotion);

        // Act
        var result =
            await _promotionService.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Black Friday", result.Name);
        Assert.Equal(25, result.Discount);
        Assert.Single(result.GameIds);
    }

    [Fact]
    public async Task ShouldThrowNotFoundException_WhenPromotionDoesNotExist()
    {
        // Arrange
        _promotionRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(99))
            .ReturnsAsync((Promotion?)null);

        // Act + Assert
        await Assert.ThrowsAsync<NotFoundException>(
            async () => await _promotionService.GetByIdAsync(99)
        );
    }

    [Fact]
    public async Task ShouldDeletePromotion_WhenPromotionExists()
    {
        // Arrange
        var promotion = new Promotion
        {
            Id = 1,
            Name = "Promotion",
            Discount = 10,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(5),
            IsActive = true,
            CreatedByUserId = 1,
            CreatedAt = DateTime.Now
        };

        _promotionRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(1))
            .ReturnsAsync(promotion);

        _promotionRepositoryMock
            .Setup(repository =>
                repository.DeleteAsync(promotion))
            .Returns(Task.CompletedTask);

        // Act
        await _promotionService.DeleteAsync(1);

        // Assert
        _promotionRepositoryMock.Verify(
            repository =>
                repository.DeleteAsync(promotion),
            Times.Once
        );
    }

    [Fact]
    public async Task ShouldThrowNotFoundException_WhenDeletingPromotionThatDoesNotExist()
    {
        // Arrange
        _promotionRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(99))
            .ReturnsAsync((Promotion?)null);

        // Act + Assert
        await Assert.ThrowsAsync<NotFoundException>(
            async () => await _promotionService.DeleteAsync(99)
        );

        _promotionRepositoryMock.Verify(
            repository =>
                repository.DeleteAsync(
                    It.IsAny<Promotion>()),
            Times.Never
        );
    }
}