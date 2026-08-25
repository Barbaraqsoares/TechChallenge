using TechChallenge.Domain.Entity;
using TechChallenge.Domain.Exceptions;
using TechChallenge.Domain.Models.Games;
using TechChallenge.Domain.Services;
using TechChallenge.Infrastructure.Repository;

namespace TechChallengeIntegrationTests;

/// <summary>
/// GameService + GameRepository + Entity Framework, sem mocks.
/// Cobre o CRUD completo e verifica que o dado realmente foi ao banco.
/// </summary>
public class GameFlowTests : IntegrationTestBase
{
    private readonly GameService _service;

    public GameFlowTests()
    {
        _service = new GameService(new GameRepository(Context));
    }

    [Fact]
    public async Task DevePersistirOJogo_QuandoCriado()
    {
        // Arrange
        var game = new CreateGameRequest
        {
            Name = "Minecraft",
            Description = "Sandbox",
            Price = 99.90m,
            IsMultiplayer = true
        };

        // Act
        var criado = await _service.CreateAsync(game);

        LimparRastreamento();

        // Assert
        var doBanco = await Context.Games.FindAsync(criado.Id);

        Assert.NotNull(doBanco);
        Assert.Equal("Minecraft", doBanco.Name);
        Assert.Equal(99.90m, doBanco.Price);
        Assert.True(doBanco.IsActive);
    }

    [Fact]
    public async Task DeveGerarIdAutomaticamente_QuandoJogoEPersistido()
    {
        // Act
        var primeiro = await _service.CreateAsync(new CreateGameRequest { Name = "Jogo A", Price = 10 });
        var segundo = await _service.CreateAsync(new CreateGameRequest { Name = "Jogo B", Price = 20 });

        // Assert
        // O Id é responsabilidade do banco — em teste de unidade com mock ele
        // ficaria sempre zero e essa regra nunca seria exercitada.
        Assert.True(primeiro.Id > 0);
        Assert.NotEqual(primeiro.Id, segundo.Id);
    }

    [Fact]
    public async Task NaoDevePersistirOJogo_QuandoDadosSaoInvalidos()
    {
        // Arrange
        var game = new CreateGameRequest { Name = "", Price = 50 };

        // Act + Assert
        await Assert.ThrowsAsync<DomainException>(() => _service.CreateAsync(game));

        LimparRastreamento();

        // A validação precisa barrar antes do SaveChanges: se passasse, o banco
        // ficaria com um registro inválido mesmo com a requisição rejeitada.
        Assert.Empty(Context.Games);
    }

    [Fact]
    public async Task DeveRecuperarOJogoGravado()
    {
        // Arrange
        var game = await DadoUmJogoAsync();

        LimparRastreamento();

        // Act
        var encontrado = await _service.GetByIdAsync(game.Id);

        // Assert
        Assert.Equal(game.Id, encontrado.Id);
        Assert.Equal("Minecraft", encontrado.Name);
    }

    [Fact]
    public async Task DeveLancarNotFound_QuandoJogoNaoEstaNoBanco()
    {
        // Act + Assert
        var excecao = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetByIdAsync(999));

        Assert.Equal("Jogo 999 não encontrado.", excecao.Message);
    }

    [Fact]
    public async Task DeveAtualizarOJogoNoBanco()
    {
        // Arrange
        var game = await DadoUmJogoAsync();

        LimparRastreamento();

        // Act
        await _service.UpdateAsync(game.Id, new UpdateGameRequest
        {
            Name = "Minecraft Deluxe",
            Description = "Edição especial",
            Price = 129.90m,
            IsMultiplayer = true,
            IsActive = true
        });

        LimparRastreamento();

        // Assert
        var doBanco = await Context.Games.FindAsync(game.Id);

        Assert.Equal("Minecraft Deluxe", doBanco!.Name);
        Assert.Equal(129.90m, doBanco.Price);
    }

    [Fact]
    public async Task DevePreservarADataDeCriacao_AoAtualizar()
    {
        // Arrange
        var game = await DadoUmJogoAsync();
        var criadoEm = game.CreatedAt;

        LimparRastreamento();

        // Act
        await _service.UpdateAsync(game.Id, new UpdateGameRequest
        {
            Name = "Outro nome",
            Price = 50,
            IsActive = true
        });

        LimparRastreamento();

        // Assert
        var doBanco = await Context.Games.FindAsync(game.Id);

        Assert.Equal(criadoEm, doBanco!.CreatedAt);
        Assert.True(doBanco.UpdatedAt >= criadoEm);
    }

    [Fact]
    public async Task DeveRemoverOJogoDoBanco()
    {
        // Arrange
        var game = await DadoUmJogoAsync();

        LimparRastreamento();

        // Act
        await _service.DeleteAsync(game.Id);

        LimparRastreamento();

        // Assert
        Assert.Null(await Context.Games.FindAsync(game.Id));
    }

    [Fact]
    public async Task DeveListarApenasOsJogosGravados()
    {
        // Arrange
        await DadoUmJogoAsync("Minecraft");
        await DadoUmJogoAsync("Stardew Valley", 34.90m);

        LimparRastreamento();

        // Act
        var jogos = await _service.GetAllAsync();

        // Assert
        Assert.Equal(2, jogos.Count());
        Assert.Contains(jogos, jogo => jogo.Name == "Stardew Valley");
    }
}
