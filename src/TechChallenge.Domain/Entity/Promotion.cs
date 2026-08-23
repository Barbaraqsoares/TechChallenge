namespace TechChallenge.Domain.Entity;

public class Promotion
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Discount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<Game> Games { get; set; } = new List<Game>();
}