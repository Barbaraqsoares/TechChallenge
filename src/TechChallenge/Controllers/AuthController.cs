using Microsoft.AspNetCore.Mvc;
using TechChallenge.Domain.Entity;
using TechChallenge.Domain.Exceptions;
using TechChallenge.Domain.Input;
using TechChallenge.Domain.Interfaces;
using TechChallenge.Domain.Output;

namespace TechChallenge.Controllers;

/// <summary>
/// Cadastro de usuários e autenticação na plataforma.
/// </summary>
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// As dependências são entregues prontas pela injeção de dependência.
    /// </summary>
    public AuthController(
        IUsuarioRepository usuarioRepository,
        ITokenService tokenService,
        ILogger<AuthController> logger)
    {
        _usuarioRepository = usuarioRepository;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// Cadastra um novo usuário na plataforma.
    /// </summary>
    /// <remarks>
    /// O e-mail precisa ter formato válido e a senha, no mínimo 8 caracteres com
    /// letras, números e caracteres especiais.
    /// </remarks>
    /// <response code="201">Usuário cadastrado.</response>
    /// <response code="400">Dados inválidos ou e-mail já cadastrado.</response>
    [HttpPost("registrar")]
    [ProducesResponseType(typeof(UsuarioOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioInput input)
    {
        if (await _usuarioRepository.ExisteComEmail(input.Email))
        {
            throw new DomainException($"Já existe um usuário cadastrado com o e-mail '{input.Email}'.");
        }

        // A validação de e-mail e senha acontece dentro do domínio: se algo estiver
        // errado, a própria entidade lança DomainException e o middleware devolve 400.
        var usuario = Usuario.Criar(input.Nome, input.Email, input.Senha);

        await _usuarioRepository.Cadastrar(usuario);

        _logger.LogInformation("Usuário {UsuarioId} cadastrado com sucesso.", usuario.Id);

        return CreatedAtAction(
            actionName: nameof(UsuariosController.ObterPorId),
            controllerName: "Usuarios",
            routeValues: new { id = usuario.Id },
            value: UsuarioOutput.De(usuario));
    }

    /// <summary>
    /// Autentica o usuário e devolve o token JWT.
    /// </summary>
    /// <remarks>
    /// Use o token retornado no botão <b>Authorize</b> desta página, no formato
    /// <c>Bearer {token}</c>, para acessar os endpoints protegidos.
    /// </remarks>
    /// <response code="200">Autenticado.</response>
    /// <response code="401">E-mail ou senha inválidos.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginInput input)
    {
        var usuario = await _usuarioRepository.ObterPorEmail(input.Email);

        // A mesma mensagem para usuário inexistente e senha errada: informar qual
        // dos dois falhou ajudaria alguém a descobrir quais e-mails existem.
        if (usuario is null || !usuario.Autenticar(input.Senha))
        {
            _logger.LogWarning("Tentativa de login malsucedida para {Email}.", input.Email);

            return Unauthorized(new { mensagem = "E-mail ou senha inválidos." });
        }

        var token = _tokenService.GerarToken(usuario);

        _logger.LogInformation("Usuário {UsuarioId} autenticado.", usuario.Id);

        return Ok(new LoginOutput
        {
            Token = token.Token,
            ExpiraEm = token.ExpiraEm,
            Usuario = UsuarioOutput.De(usuario)
        });
    }
}
