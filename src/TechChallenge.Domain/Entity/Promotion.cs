namespace TechChallenge.Domain.Entity;

public class Promotion
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Discout { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public int GameId { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdateAt { get; set; }
}