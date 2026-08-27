namespace TechChallenge.Domain.Models.Games;

/// <summary>
/// Dados que o cliente informa ao cadastrar um jogo.
///
/// Id, CreatedAt e UpdatedAt não entram aqui de propósito: são responsabilidade do
/// banco e do serviço. Promoções também não — um jogo não cria promoções, elas são
/// cadastradas pelo endpoint próprio, que valida desconto, período e jogos.
/// </summary>
public class CreateGameRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool IsMultiplayer { get; set; }
}
