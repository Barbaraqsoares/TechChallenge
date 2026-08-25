namespace TechChallenge.Domain.Entity;

public class Game : EntityBase
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public bool IsMultiplayer { get; set; }
    public ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();
}
