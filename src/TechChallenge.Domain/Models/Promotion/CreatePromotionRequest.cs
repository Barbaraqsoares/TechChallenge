namespace TechChallenge.Domain.Models.Promotion;

public class CreatePromotionRequest
{
    public required string Name { get; set; }
    public decimal Discount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<int> GameIds { get; set; } = new();
}