namespace TechChallenge.Domain.Models.Games;

/// <summary>
/// Jogo como a API o devolve.
///
/// Não expõe a coleção de promoções da entidade: além de criar referência circular
/// na serialização, ela traria dados que o consumidor do catálogo não pediu.
/// </summary>
public class GameResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public bool IsMultiplayer { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
