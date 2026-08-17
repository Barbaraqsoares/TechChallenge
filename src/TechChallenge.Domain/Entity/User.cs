using System.Net.NetworkInformation;

namespace TechChallenge.Domain.Entity;

public class User : EntityBAse
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string Login { get; set; }
    public required string Password { get; set; }
    public PerfilEnum Perfil { get; set; }

    /*public ICollection<Game> Games { get; set; } = new List<Games>();*/

    public User(string name, PerfilEnum perfil, string email, string password, string login)
    {
        Name = name.Trim();
        Perfil = perfil;
        Email = email;
        Password = password;
        Login = login;
    }

    public void Update(string name, string email, string password)
    {
        Name = name.Trim();
        Name = name.Trim();
        Email = email;
        Password = password;
    }
}
