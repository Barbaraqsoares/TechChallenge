using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechChallenge.Domain.Entity;
using TechChallenge.Domain.Exceptions;
using TechChallenge.Domain.Input;
using TechChallenge.Domain.Interfaces;
using TechChallenge.Domain.Output;

namespace TechChallenge.Controllers;

/// <summary>
/// Usuários da plataforma e sua biblioteca de jogos.
/// </summary>
[ApiController]
[Route("[controller]")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IJogoRepository _jogoRepository;
    private readonly ILogger<UsuariosController> _logger;

    /// <summary>
    /// As dependências são entregues prontas pela injeção de dependência.
    /// </summary>
    public UsuariosController(
        IUsuarioRepository usuarioRepository,
        IJogoRepository jogoRepository,
        ILogger<UsuariosController> logger)
    {
        _usuarioRepository = usuarioRepository;
        _jogoRepository = jogoRepository;
        _logger = logger;
    }

    /// <summary>
    /// Lista todos os usuários. Exclusivo para administradores.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = nameof(PerfilEnum.Admin))]
    [ProducesResponseType(typeof(IEnumerable<UsuarioOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ObterTodos()
    {
        var usuarios = await _usuarioRepository.ObterTodos();

        return Ok(usuarios.Select(UsuarioOutput.De));
    }

    /// <summary>
    /// Busca um usuário pelo identificador.
    /// </summary>
    /// <remarks>
    /// Um usuário comum só consegue consultar os próprios dados; administradores
    /// consultam qualquer um.
    /// </remarks>
    /// <response code="403">Tentativa de consultar outro usuário sem ser administrador.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UsuarioOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId([FromRoute] int id)
    {
        if (!PodeAcessarDadosDe(id))
        {
            return Forbid();
        }

        var usuario = await _usuarioRepository.ObterPorId(id)
            ?? throw new NotFoundException($"Usuário {id} não encontrado.");

        return Ok(UsuarioOutput.De(usuario));
    }

    /// <summary>
    /// Retorna os dados do usuário autenticado.
    /// </summary>
    [HttpGet("meu-perfil")]
    [ProducesResponseType(typeof(UsuarioOutput), StatusCodes.Status200OK)]
    public async Task<IActionResult> MeuPerfil()
    {
        var usuario = await _usuarioRepository.ObterPorId(UsuarioAutenticadoId())
            ?? throw new NotFoundException("Usuário autenticado não encontrado.");

        return Ok(UsuarioOutput.De(usuario));
    }

    /// <summary>
    /// Lista a biblioteca de jogos adquiridos pelo usuário.
    /// </summary>
    [HttpGet("{id:int}/biblioteca")]
    [ProducesResponseType(typeof(IEnumerable<JogoAdquiridoOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterBiblioteca([FromRoute] int id)
    {
        if (!PodeAcessarDadosDe(id))
        {
            return Forbid();
        }

        var usuario = await _usuarioRepository.ObterComBiblioteca(id)
            ?? throw new NotFoundException($"Usuário {id} não encontrado.");

        return Ok(usuario.Biblioteca.Select(JogoAdquiridoOutput.De));
    }

    /// <summary>
    /// Adquire um jogo e o adiciona à biblioteca do usuário autenticado.
    /// </summary>
    /// <response code="400">O jogo já está na biblioteca.</response>
    /// <response code="404">Jogo não encontrado.</response>
    [HttpPost("meus-jogos/{jogoId:int}")]
    [ProducesResponseType(typeof(JogoAdquiridoOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdquirirJogo([FromRoute] int jogoId)
    {
        var usuarioId = UsuarioAutenticadoId();

        var usuario = await _usuarioRepository.ObterComBiblioteca(usuarioId)
            ?? throw new NotFoundException("Usuário autenticado não encontrado.");

        var jogo = await _jogoRepository.ObterPorId(jogoId)
            ?? throw new NotFoundException($"Jogo {jogoId} não encontrado.");

        // A regra "não pode comprar duas vezes" vive na entidade, não aqui.
        var aquisicao = usuario.AdquirirJogo(jogo);

        await _usuarioRepository.Alterar(usuario);

        _logger.LogInformation(
            "Usuário {UsuarioId} adquiriu o jogo {JogoId} por {PrecoPago}.",
            usuarioId,
            jogoId,
            aquisicao.PrecoPago);

        return CreatedAtAction(
            nameof(ObterBiblioteca),
            new { id = usuarioId },
            JogoAdquiridoOutput.De(aquisicao));
    }

    /// <summary>
    /// Altera o nível de acesso de um usuário. Exclusivo para administradores.
    /// </summary>
    [HttpPatch("{id:int}/perfil")]
    [Authorize(Roles = nameof(PerfilEnum.Admin))]
    [ProducesResponseType(typeof(UsuarioOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AlterarPerfil([FromRoute] int id, [FromBody] AlterarPerfilInput input)
    {
        var usuario = await _usuarioRepository.ObterPorId(id)
            ?? throw new NotFoundException($"Usuário {id} não encontrado.");

        usuario.AlterarPerfil(input.Perfil);

        await _usuarioRepository.Alterar(usuario);

        _logger.LogInformation(
            "Perfil do usuário {UsuarioId} alterado para {Perfil}.",
            id,
            input.Perfil);

        return Ok(UsuarioOutput.De(usuario));
    }

    /// <summary>
    /// Remove um usuário. Exclusivo para administradores.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(PerfilEnum.Admin))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deletar([FromRoute] int id)
    {
        _ = await _usuarioRepository.ObterPorId(id)
            ?? throw new NotFoundException($"Usuário {id} não encontrado.");

        await _usuarioRepository.Deletar(id);

        return NoContent();
    }

    /// <summary>
    /// Id do usuário que está no token JWT da requisição.
    /// </summary>
    private int UsuarioAutenticadoId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Um usuário só acessa os próprios dados; o administrador acessa os de todos.
    /// </summary>
    private bool PodeAcessarDadosDe(int usuarioId) =>
        User.IsInRole(nameof(PerfilEnum.Admin)) || UsuarioAutenticadoId() == usuarioId;
}
