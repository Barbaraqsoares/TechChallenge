namespace TechChallenge.Domain.Entity;

public class Game
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public bool IsMultiplayer { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();
}