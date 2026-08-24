namespace TechChallenge.Domain.Models.UserGame;

public class UserGameResponse
{
    public int GameId { get; set; }
    public string GameName { get; set; }
    public decimal Price { get; set; }
    public DateTime PurchasedAt { get; set; }
}