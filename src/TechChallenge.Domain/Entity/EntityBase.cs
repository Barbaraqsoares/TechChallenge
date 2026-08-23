using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TechChallenge.Domain.Entity;

public class EntityBAse
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}