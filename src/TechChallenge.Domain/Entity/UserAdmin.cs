namespace TechChallenge.Domain.Entity;

public class UserAdmin : UserBase
{
    public required string Name { get; set; }
    public required string Email { get; set; }

    public UserAdmin(string name, PerfilEnum perfil, string email, string password, string login)
        : base(login, password)
    {
        Name = name.Trim();
        Perfil = perfil;
        Email = email;
        Password = password;
        Login = login;
    }

    public void Update(string name, PerfilEnum perfil, string email, string password)
    {
        Name = name.Trim();
        Perfil = perfil;
        Email = email;
        Password = password;
    }
}