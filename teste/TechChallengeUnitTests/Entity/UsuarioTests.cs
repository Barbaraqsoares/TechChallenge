using TechChallenge.Domain.Entity;
using TechChallenge.Domain.Exceptions;

namespace TechChallengeUnitTests.Entity;

/// <summary>
/// Testes das regras de negócio do usuário e da sua biblioteca de jogos.
/// </summary>
public class UsuarioTests
{
    private static Usuario CriarUsuarioValido() =>
        Usuario.Criar("Maria Silva", "maria@fiap.com.br", "Fiap@2026");

    private static Jogo CriarJogoValido(decimal preco = 100m) =>
        Jogo.Criar("The Legend of FIAP", "Aventura pela tecnologia", preco, "Aventura", new DateTime(2026, 1, 15));

    [Fact]
    public void Criar_ComDadosValidos_DeveNascerComPerfilCliente()
    {
        // Act
        var usuario = CriarUsuarioValido();

        // Assert — quem se cadastra sozinho nunca vira administrador.
        Assert.Equal(PerfilEnum.Cliente, usuario.Perfil);
        Assert.Equal("Maria Silva", usuario.Nome);
        Assert.Equal("maria@fiap.com.br", usuario.Email.Endereco);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_SemNome_DeveLancarExcecao(string nomeVazio)
    {
        // Act
        var excecao = Assert.Throws<DomainException>(
            () => Usuario.Criar(nomeVazio, "maria@fiap.com.br", "Fiap@2026"));

        // Assert
        Assert.Equal("O nome é obrigatório.", excecao.Message);
    }

    [Fact]
    public void Criar_DeveNascerComBibliotecaVazia()
    {
        // Act
        var usuario = CriarUsuarioValido();

        // Assert
        Assert.Empty(usuario.Biblioteca);
    }

    [Fact]
    public void Autenticar_ComSenhaCorreta_DeveRetornarVerdadeiro()
    {
        // Arrange
        var usuario = CriarUsuarioValido();

        // Act & Assert
        Assert.True(usuario.Autenticar("Fiap@2026"));
        Assert.False(usuario.Autenticar("SenhaErrada@1"));
    }

    [Fact]
    public void AdquirirJogo_DeveIncluirNaBibliotecaPeloPrecoAtual()
    {
        // Arrange
        var usuario = CriarUsuarioValido();
        var jogo = CriarJogoValido(preco: 200m);
        jogo.AplicarPromocao(25);

        // Act
        var aquisicao = usuario.AdquirirJogo(jogo);

        // Assert — o preço pago é o promocional, não o de tabela.
        Assert.Single(usuario.Biblioteca);
        Assert.Equal(150m, aquisicao.PrecoPago);
    }

    [Fact]
    public void AdquirirJogo_DuasVezes_DeveLancarExcecao()
    {
        // Arrange
        var usuario = CriarUsuarioValido();
        var jogo = CriarJogoValido();
        usuario.AdquirirJogo(jogo);

        // Act
        var excecao = Assert.Throws<DomainException>(() => usuario.AdquirirJogo(jogo));

        // Assert
        Assert.Contains("já está na sua biblioteca", excecao.Message);
        Assert.Single(usuario.Biblioteca);
    }

    [Fact]
    public void AlterarPerfil_DevePromoverParaAdministrador()
    {
        // Arrange
        var usuario = CriarUsuarioValido();

        // Act
        usuario.AlterarPerfil(PerfilEnum.Admin);

        // Assert
        Assert.Equal(PerfilEnum.Admin, usuario.Perfil);
    }

    [Fact]
    public void AlterarSenha_DevePassarPelaMesmaValidacao()
    {
        // Arrange
        var usuario = CriarUsuarioValido();

        // Act & Assert — senha fraca continua sendo recusada na troca.
        Assert.Throws<DomainException>(() => usuario.AlterarSenha("123"));

        usuario.AlterarSenha("NovaSenha@2026");
        Assert.True(usuario.Autenticar("NovaSenha@2026"));
    }
}
