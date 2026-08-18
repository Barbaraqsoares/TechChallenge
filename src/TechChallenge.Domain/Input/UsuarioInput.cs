using TechChallenge.Domain.Entity;

namespace TechChallenge.Domain.Input;

/// <summary>
/// Dados para cadastro de um novo usuário.
/// </summary>
public class RegistrarUsuarioInput
{
    /// <summary>Nome completo do usuário.</summary>
    /// <example>Maria Silva</example>
    public required string Nome { get; set; }

    /// <summary>E-mail usado para login.</summary>
    /// <example>maria.silva@fiap.com.br</example>
    public required string Email { get; set; }

    /// <summary>Mínimo de 8 caracteres, com letras, números e caracteres especiais.</summary>
    /// <example>Fiap@2026</example>
    public required string Senha { get; set; }
}

/// <summary>
/// Credenciais de acesso.
/// </summary>
public class LoginInput
{
    /// <example>maria.silva@fiap.com.br</example>
    public required string Email { get; set; }

    /// <example>Fiap@2026</example>
    public required string Senha { get; set; }
}

/// <summary>
/// Alteração do nível de acesso de um usuário (somente administradores).
/// </summary>
public class AlterarPerfilInput
{
    public required PerfilEnum Perfil { get; set; }
}
