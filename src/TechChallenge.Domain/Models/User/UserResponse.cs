using TechChallenge.Domain.Entity;

namespace TechChallenge.Domain.Models.User;

public class UserResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Login { get; set; }
    public PerfilEnum Perfil { get; set; }
    public DateTime CreatedAt { get; set; }
}