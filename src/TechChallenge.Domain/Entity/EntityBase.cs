namespace TechChallenge.Domain.Entity;

public class EntityBase
{
    public int Id { get; set; }
    public PerfilEnum Perfil { get; set; }
    public DateTime CreatedAt { get; set; }
}