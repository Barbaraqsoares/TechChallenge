namespace TechChallenge.Domain.Models.Games;

/// <summary>
/// Dados que o cliente informa ao atualizar um jogo.
///
/// O id vem pela rota, não pelo corpo. IsActive aparece aqui — diferente do cadastro,
/// porque é na atualização que o jogo pode ser tirado ou devolvido ao catálogo.
/// </summary>
public class UpdateGameRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool IsMultiplayer { get; set; }
    public bool IsActive { get; set; }
}
