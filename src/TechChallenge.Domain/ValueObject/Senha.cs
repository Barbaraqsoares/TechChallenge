using System.Security.Cryptography;
using TechChallenge.Domain.Exceptions;

namespace TechChallenge.Domain.ValueObject;

/// <summary>
/// Objeto de Valor que representa a senha do usuário, já protegida por hash.
///
/// A senha em texto puro nunca é armazenada: guardamos apenas o resultado do
/// algoritmo PBKDF2 (System.Security.Cryptography, nativo do .NET) com um salt
/// aleatório por usuário. Assim, mesmo que o banco vaze, as senhas não são
/// reveladas — e dois usuários com a mesma senha geram hashes diferentes.
/// </summary>
public sealed class Senha
{
    // Requisito do desafio: mínimo de 8 caracteres, com letras, números e especiais.
    public const int TamanhoMinimo = 8;

    private const int TamanhoDoSalt = 16;      // 128 bits
    private const int TamanhoDoHash = 32;      // 256 bits
    private const int Iteracoes = 100_000;

    /// <summary>
    /// Salt e hash concatenados em Base64. É este valor que vai para o banco.
    /// </summary>
    public string Hash { get; }

    private Senha(string hash) => Hash = hash;

    /// <summary>
    /// Valida a política de segurança e devolve a senha já com hash.
    /// </summary>
    public static Senha Criar(string senhaEmTextoPuro)
    {
        ValidarPolitica(senhaEmTextoPuro);

        var salt = RandomNumberGenerator.GetBytes(TamanhoDoSalt);
        var hash = CalcularHash(senhaEmTextoPuro, salt);

        // Guardamos salt + hash juntos para conseguir conferir a senha depois.
        var saltComHash = new byte[TamanhoDoSalt + TamanhoDoHash];
        salt.CopyTo(saltComHash, 0);
        hash.CopyTo(saltComHash, TamanhoDoSalt);

        return new Senha(Convert.ToBase64String(saltComHash));
    }

    /// <summary>
    /// Reconstrói o objeto a partir do hash já gravado no banco.
    /// Usado apenas pelo Entity Framework na leitura — não valida a política,
    /// porque o valor lido já passou por ela quando foi criado.
    /// </summary>
    public static Senha APartirDoHash(string hash) => new(hash);

    /// <summary>
    /// Confere se a senha informada no login corresponde a esta senha.
    /// </summary>
    public bool Conferir(string senhaEmTextoPuro)
    {
        if (string.IsNullOrEmpty(senhaEmTextoPuro))
        {
            return false;
        }

        var saltComHash = Convert.FromBase64String(Hash);

        var salt = saltComHash[..TamanhoDoSalt];
        var hashArmazenado = saltComHash[TamanhoDoSalt..];

        var hashInformado = CalcularHash(senhaEmTextoPuro, salt);

        // Comparação em tempo fixo: não revela quantos bytes bateram.
        return CryptographicOperations.FixedTimeEquals(hashArmazenado, hashInformado);
    }

    /// <summary>
    /// Regras de senha segura exigidas pelo desafio. Cada regra tem sua própria
    /// mensagem para que o usuário saiba exatamente o que corrigir.
    /// </summary>
    private static void ValidarPolitica(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha))
        {
            throw new DomainException("A senha é obrigatória.");
        }

        if (senha.Length < TamanhoMinimo)
        {
            throw new DomainException($"A senha deve ter no mínimo {TamanhoMinimo} caracteres.");
        }

        if (!senha.Any(char.IsLetter))
        {
            throw new DomainException("A senha deve conter ao menos uma letra.");
        }

        if (!senha.Any(char.IsDigit))
        {
            throw new DomainException("A senha deve conter ao menos um número.");
        }

        // Um caractere especial é tudo que não é letra nem número.
        if (senha.All(char.IsLetterOrDigit))
        {
            throw new DomainException("A senha deve conter ao menos um caractere especial.");
        }
    }

    private static byte[] CalcularHash(string senha, byte[] salt) =>
        Rfc2898DeriveBytes.Pbkdf2(senha, salt, Iteracoes, HashAlgorithmName.SHA256, TamanhoDoHash);
}
