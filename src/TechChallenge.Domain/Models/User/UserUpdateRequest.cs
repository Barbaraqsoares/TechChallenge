using TechChallenge.Domain.Entity;

namespace TechChallenge.Domain.Models.User;

public record UserUpdateRequest(string Name, string Email, string Login, PerfilEnum Perfil);
