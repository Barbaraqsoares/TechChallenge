namespace TechChallenge.Domain.Entity;

public class UserGame : EntityBase
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int GameId { get; set; }
    public Game Game { get; set; } = null!;

    public DateTime PurchasedAt { get; set; } = DateTime.Now;
}