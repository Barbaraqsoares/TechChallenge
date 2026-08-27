using TechChallenge.Domain.Exceptions;
using TechChallenge.Domain.Models.Promotion;
using TechChallenge.Domain.Services;
using TechChallenge.Infrastructure.Repository;

namespace TechChallengeIntegrationTests;

/// <summary>
/// Promoções — o caso com relacionamento muitos-para-muitos entre promoção e jogos,
/// que só é exercitado de verdade contra o Entity Framework.
/// </summary>
public class PromotionFlowTests : IntegrationTestBase
{
    private readonly PromotionService _service;

    public PromotionFlowTests()
    {
        _service = new PromotionService(
            new PromotionRepository(Context),
            new GameRepository(Context));
    }

    [Fact]
    public async Task DevePersistirAPromocaoComOsJogosVinculados()
    {
        // Arrange
        var minecraft = await DadoUmJogoAsync("Minecraft");
        var stardew = await DadoUmJogoAsync("Stardew Valley", 34.90m);

        LimparRastreamento();

        // Act
        var criada = await _service.CreateAsync(new CreatePromotionRequest
        {
            Name = "Black Friday",
            Discount = 25,
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(10),
            GameIds = new List<int> { minecraft.Id, stardew.Id }
        }, adminUserId: 1);

        LimparRastreamento();

        // Assert
        Assert.True(criada.Id > 0);
        Assert.Equal(2, criada.GameIds.Count);

        // O vínculo precisa ter chegado ao banco, não apenas ao objeto devolvido.
        var doBanco = await _service.GetByIdAsync(criada.Id);

        Assert.Equal(2, doBanco.GameIds.Count);
        Assert.Contains(minecraft.Id, doBanco.GameIds);
    }

    [Fact]
    public async Task DeveRecusarAPromocao_QuandoAlgumJogoNaoExiste()
    {
        // Arrange
        var minecraft = await DadoUmJogoAsync();

        LimparRastreamento();

        // Act + Assert
        var excecao = await Assert.ThrowsAsync<DomainException>(
            () => _service.CreateAsync(new CreatePromotionRequest
            {
                Name = "Promoção inválida",
                Discount = 25,
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(10),
                GameIds = new List<int> { minecraft.Id, 999 }
            }, adminUserId: 1));

        Assert.Equal("Um ou mais games não foram encontrados.", excecao.Message);

        LimparRastreamento();

        Assert.Empty(Context.Promotions);
    }

    [Fact]
    public async Task NaoDevePersistirAPromocao_QuandoDadosSaoInvalidos()
    {
        // Arrange
        var game = await DadoUmJogoAsync();

        LimparRastreamento();

        // Act + Assert
        await Assert.ThrowsAsync<DomainException>(
            () => _service.CreateAsync(new CreatePromotionRequest
            {
                Name = "Desconto absurdo",
                Discount = 150,
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(10),
                GameIds = new List<int> { game.Id }
            }, adminUserId: 1));

        LimparRastreamento();

        Assert.Empty(Context.Promotions);
    }

    [Fact]
    public async Task DeveLancarNotFound_QuandoPromocaoNaoExiste()
    {
        // Act + Assert
        var excecao = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetByIdAsync(999));

        Assert.Equal("Promoção 999 não encontrada.", excecao.Message);
    }

    [Fact]
    public async Task DeveRemoverAPromocaoSemApagarOsJogos()
    {
        // Arrange
        var game = await DadoUmJogoAsync();

        var criada = await _service.CreateAsync(new CreatePromotionRequest
        {
            Name = "Promoção temporária",
            Discount = 10,
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(5),
            GameIds = new List<int> { game.Id }
        }, adminUserId: 1);

        LimparRastreamento();

        // Act
        await _service.DeleteAsync(criada.Id);

        LimparRastreamento();

        // Assert
        Assert.Empty(Context.Promotions);

        // Apagar a promoção não pode levar junto o jogo que estava nela.
        Assert.Single(Context.Games);
    }

    [Fact]
    public async Task DeveLancarNotFound_AoRemoverPromocaoInexistente()
    {
        // Act + Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(999));
    }

    [Fact]
    public async Task DeveListarTodasAsPromocoesGravadas()
    {
        // Arrange
        var game = await DadoUmJogoAsync();

        await _service.CreateAsync(new CreatePromotionRequest
        {
            Name = "Promoção A",
            Discount = 10,
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(5),
            GameIds = new List<int> { game.Id }
        }, adminUserId: 1);

        await _service.CreateAsync(new CreatePromotionRequest
        {
            Name = "Promoção B",
            Discount = 20,
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(5),
            GameIds = new List<int> { game.Id }
        }, adminUserId: 1);

        LimparRastreamento();

        // Act
        var promocoes = await _service.GetAllAsync();

        // Assert
        Assert.Equal(2, promocoes.Count());
        Assert.Contains(promocoes, promocao => promocao.Name == "Promoção B");
    }
}
