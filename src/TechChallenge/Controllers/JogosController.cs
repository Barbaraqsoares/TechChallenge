using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechChallenge.Domain.Entity;
using TechChallenge.Domain.Exceptions;
using TechChallenge.Domain.Input;
using TechChallenge.Domain.Interfaces;
using TechChallenge.Domain.Output;

namespace TechChallenge.Controllers;

/// <summary>
/// Catálogo de jogos. Consultar é liberado para qualquer usuário autenticado;
/// cadastrar, alterar e criar promoções é exclusivo do administrador.
/// </summary>
[ApiController]
[Route("[controller]")]
[Authorize]
public class JogosController : ControllerBase
{
    private readonly IJogoRepository _jogoRepository;
    private readonly ILogger<JogosController> _logger;

    /// <summary>
    /// As dependências são entregues prontas pela injeção de dependência.
    /// </summary>
    public JogosController(IJogoRepository jogoRepository, ILogger<JogosController> logger)
    {
        _jogoRepository = jogoRepository;
        _logger = logger;
    }

    /// <summary>
    /// Lista todos os jogos do catálogo.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<JogoOutput>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterTodos()
    {
        var jogos = await _jogoRepository.ObterTodos();

        return Ok(jogos.Select(JogoOutput.De));
    }

    /// <summary>
    /// Lista apenas os jogos com promoção vigente.
    /// </summary>
    [HttpGet("promocoes")]
    [ProducesResponseType(typeof(IEnumerable<JogoOutput>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterEmPromocao()
    {
        var jogos = await _jogoRepository.ObterEmPromocao();

        return Ok(jogos.Select(JogoOutput.De));
    }

    /// <summary>
    /// Lista os jogos de um gênero.
    /// </summary>
    [HttpGet("genero/{genero}")]
    [ProducesResponseType(typeof(IEnumerable<JogoOutput>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterPorGenero([FromRoute] string genero)
    {
        var jogos = await _jogoRepository.ObterPorGenero(genero);

        return Ok(jogos.Select(JogoOutput.De));
    }

    /// <summary>
    /// Busca um jogo pelo identificador.
    /// </summary>
    /// <response code="404">Jogo não encontrado.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(JogoOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId([FromRoute] int id)
    {
        var jogo = await _jogoRepository.ObterPorId(id)
            ?? throw new NotFoundException($"Jogo {id} não encontrado.");

        return Ok(JogoOutput.De(jogo));
    }

    /// <summary>
    /// Cadastra um novo jogo no catálogo. Exclusivo para administradores.
    /// </summary>
    /// <response code="403">Usuário autenticado, mas sem perfil de administrador.</response>
    [HttpPost]
    [Authorize(Roles = nameof(PerfilEnum.Admin))]
    [ProducesResponseType(typeof(JogoOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Cadastrar([FromBody] JogoInput input)
    {
        var jogo = Jogo.Criar(
            input.Titulo,
            input.Descricao,
            input.Preco,
            input.Genero,
            input.DataLancamento);

        await _jogoRepository.Cadastrar(jogo);

        _logger.LogInformation("Jogo {JogoId} cadastrado por um administrador.", jogo.Id);

        return CreatedAtAction(nameof(ObterPorId), new { id = jogo.Id }, JogoOutput.De(jogo));
    }

    /// <summary>
    /// Atualiza os dados de um jogo. Exclusivo para administradores.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(PerfilEnum.Admin))]
    [ProducesResponseType(typeof(JogoOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Alterar([FromRoute] int id, [FromBody] JogoInput input)
    {
        var jogo = await _jogoRepository.ObterPorId(id)
            ?? throw new NotFoundException($"Jogo {id} não encontrado.");

        jogo.Atualizar(
            input.Titulo,
            input.Descricao,
            input.Preco,
            input.Genero,
            input.DataLancamento);

        await _jogoRepository.Alterar(jogo);

        return Ok(JogoOutput.De(jogo));
    }

    /// <summary>
    /// Cria ou encerra a promoção de um jogo. Exclusivo para administradores.
    /// </summary>
    /// <remarks>Informe 0 no percentual para encerrar a promoção.</remarks>
    [HttpPatch("{id:int}/promocao")]
    [Authorize(Roles = nameof(PerfilEnum.Admin))]
    [ProducesResponseType(typeof(JogoOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AplicarPromocao([FromRoute] int id, [FromBody] PromocaoInput input)
    {
        var jogo = await _jogoRepository.ObterPorId(id)
            ?? throw new NotFoundException($"Jogo {id} não encontrado.");

        jogo.AplicarPromocao(input.PercentualDesconto);

        await _jogoRepository.Alterar(jogo);

        _logger.LogInformation(
            "Promoção de {PercentualDesconto}% aplicada ao jogo {JogoId}.",
            input.PercentualDesconto,
            jogo.Id);

        return Ok(JogoOutput.De(jogo));
    }

    /// <summary>
    /// Remove um jogo do catálogo. Exclusivo para administradores.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(PerfilEnum.Admin))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deletar([FromRoute] int id)
    {
        _ = await _jogoRepository.ObterPorId(id)
            ?? throw new NotFoundException($"Jogo {id} não encontrado.");

        await _jogoRepository.Deletar(id);

        return NoContent();
    }
}
