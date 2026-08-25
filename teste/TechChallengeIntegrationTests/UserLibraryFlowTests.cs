using TechChallenge.Domain.Exceptions;
using TechChallenge.Domain.Services;
using TechChallenge.Infrastructure.Repository;

namespace TechChallengeIntegrationTests;

/// <summary>
/// Fluxo de biblioteca do usuário — o caso que mais depende de integração, porque
/// envolve três repositórios e um relacionamento entre tabelas.
/// </summary>
public class UserLibraryFlowTests : IntegrationTestBase
{
    private readonly UserGameService _service;

    public UserLibraryFlowTests()
    {
        _service = new UserGameService(
            new UserGameRepository(Context),
            new UserRepository(Context),
            new GameRepository(Context));
    }

    [Fact]
    public async Task DeveAdicionarOJogoNaBibliotecaDoUsuario()
    {
        // Arrange
        var user = await DadoUmUsuarioAsync();
        var game = await DadoUmJogoAsync();

        LimparRastreamento();

        // Act
        var resultado = await _service.AddGameToLibraryAsync(user.Id, game.Id);

        LimparRastreamento();

        // Assert
        Assert.Equal(game.Id, resultado.GameId);
        Assert.Equal("Minecraft", resultado.GameName);

        Assert.Single(Context.UserGames);
    }

    [Fact]
    public async Task DeveLancarConflito_QuandoOJogoJaEstaNaBiblioteca()
    {
        // Arrange
        var user = await DadoUmUsuarioAsync();
        var game = await DadoUmJogoAsync();

        await _service.AddGameToLibraryAsync(user.Id, game.Id);

        LimparRastreamento();

        // Act + Assert
        var excecao = await Assert.ThrowsAsync<ConflictException>(
            () => _service.AddGameToLibraryAsync(user.Id, game.Id));

        Assert.Equal("O jogo já está na biblioteca do usuário.", excecao.Message);

        LimparRastreamento();

        // A tentativa recusada não pode ter gravado uma segunda linha.
        Assert.Single(Context.UserGames);
    }

    [Fact]
    public async Task DeveLancarNotFound_QuandoUsuarioNaoExiste()
    {
        // Arrange
        var game = await DadoUmJogoAsync();

        LimparRastreamento();

        // Act + Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.AddGameToLibraryAsync(999, game.Id));

        Assert.Empty(Context.UserGames);
    }

    [Fact]
    public async Task DeveLancarNotFound_QuandoJogoNaoExiste()
    {
        // Arrange
        var user = await DadoUmUsuarioAsync();

        LimparRastreamento();

        // Act + Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.AddGameToLibraryAsync(user.Id, 999));

        Assert.Empty(Context.UserGames);
    }

    [Fact]
    public async Task DeveTrazerOsDadosDoJogo_AoListarABiblioteca()
    {
        // Arrange
        var user = await DadoUmUsuarioAsync();
        var minecraft = await DadoUmJogoAsync("Minecraft");
        var stardew = await DadoUmJogoAsync("Stardew Valley", 34.90m);

        await _service.AddGameToLibraryAsync(user.Id, minecraft.Id);
        await _service.AddGameToLibraryAsync(user.Id, stardew.Id);

        LimparRastreamento();

        // Act
        var biblioteca = await _service.GetUserLibraryAsync(user.Id);

        // Assert
        // O nome e o preço vêm da tabela de jogos, por Include. Com o repositório
        // mockado esse join nunca é exercitado — aqui ele é.
        Assert.Equal(2, biblioteca.Count);
        Assert.Contains(biblioteca, item => item.GameName == "Stardew Valley" && item.Price == 34.90m);
        Assert.DoesNotContain(biblioteca, item => string.IsNullOrEmpty(item.GameName));
    }

    [Fact]
    public async Task DeveIsolarAsBibliotecasDeCadaUsuario()
    {
        // Arrange
        var gabriela = await DadoUmUsuarioAsync("gabriela", "gabriela@email.com");
        var micael = await DadoUmUsuarioAsync("micael", "micael@email.com");

        var minecraft = await DadoUmJogoAsync("Minecraft");
        var stardew = await DadoUmJogoAsync("Stardew Valley", 34.90m);

        await _service.AddGameToLibraryAsync(gabriela.Id, minecraft.Id);
        await _service.AddGameToLibraryAsync(micael.Id, stardew.Id);

        LimparRastreamento();

        // Act
        var bibliotecaDaGabriela = await _service.GetUserLibraryAsync(gabriela.Id);
        var bibliotecaDoMicael = await _service.GetUserLibraryAsync(micael.Id);

        // Assert
        Assert.Equal("Minecraft", Assert.Single(bibliotecaDaGabriela).GameName);
        Assert.Equal("Stardew Valley", Assert.Single(bibliotecaDoMicael).GameName);
    }

    [Fact]
    public async Task DevePermitirQueDoisUsuariosTenhamOMesmoJogo()
    {
        // Arrange
        var gabriela = await DadoUmUsuarioAsync("gabriela", "gabriela@email.com");
        var micael = await DadoUmUsuarioAsync("micael", "micael@email.com");

        var game = await DadoUmJogoAsync();

        // Act
        await _service.AddGameToLibraryAsync(gabriela.Id, game.Id);
        await _service.AddGameToLibraryAsync(micael.Id, game.Id);

        LimparRastreamento();

        // Assert
        // O conflito é por par usuário+jogo, não pelo jogo: o mesmo título pode
        // estar na biblioteca de quantos usuários for.
        Assert.Equal(2, Context.UserGames.Count());
    }

    [Fact]
    public async Task DeveRetornarBibliotecaVazia_QuandoUsuarioNaoTemJogos()
    {
        // Arrange
        var user = await DadoUmUsuarioAsync();

        LimparRastreamento();

        // Act
        var biblioteca = await _service.GetUserLibraryAsync(user.Id);

        // Assert
        Assert.Empty(biblioteca);
    }

    [Fact]
    public async Task DeveLancarNotFound_AoListarBibliotecaDeUsuarioInexistente()
    {
        // Act + Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetUserLibraryAsync(999));
    }
}
