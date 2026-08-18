using System.Text.RegularExpressions;
using TechChallenge.Domain.Exceptions;

namespace TechChallenge.Domain.ValueObject;

/// <summary>
/// Objeto de Valor que representa um e-mail válido.
///
/// No DDD, um Objeto de Valor não tem identidade própria: ele é definido pelo
/// seu conteúdo e é imutável. Dois e-mails com o mesmo texto são o mesmo e-mail.
/// Vantagem prática: depois que um Email existe, ele é necessariamente válido —
/// não há como criar um inválido.
/// </summary>
public sealed record Email
{
    /// <summary>
    /// Formato aceito: algo@dominio.extensao (extensão com 2 letras ou mais).
    /// </summary>
    private static readonly Regex Formato = new(
        @"^[^@\s]+@[^@\s]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled);

    public string Endereco { get; }

    private Email(string endereco) => Endereco = endereco;

    /// <summary>
    /// Cria o e-mail validando o formato exigido pelo desafio.
    /// </summary>
    public static Email Criar(string endereco)
    {
        if (string.IsNullOrWhiteSpace(endereco))
        {
            throw new DomainException("O e-mail é obrigatório.");
        }

        // Normaliza para que "JOAO@FIAP.COM" e "joao@fiap.com" sejam o mesmo e-mail.
        var normalizado = endereco.Trim().ToLowerInvariant();

        if (!Formato.IsMatch(normalizado))
        {
            throw new DomainException($"O e-mail '{endereco}' não é válido.");
        }

        return new Email(normalizado);
    }

    public override string ToString() => Endereco;
}
