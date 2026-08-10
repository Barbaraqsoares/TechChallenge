namespace TechChallenge.Domain.Entity;

public class User : EntityBase
{
    public PerfilEnum Perfil { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }

    public User(string name, PerfilEnum perfil, string email, string password)
    {
        Name = name.Trim();
        Perfil = perfil;
        Email = email;
        Password = password;
    }

    public void Update(string name, PerfilEnum perfil, string email, string password)
    {
        Name = name.Trim();
        Perfil = perfil;
        Email = email;
        Password = password;
    }
}
