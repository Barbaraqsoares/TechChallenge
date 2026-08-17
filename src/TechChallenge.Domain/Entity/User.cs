namespace TechChallenge.Domain.Entity;

public class User : UserBase
{
    public string Name { get; set; }
    public string Email { get; set; }

    public User(string name, PerfilEnum perfil, string email, string password, string login)
        : base(login, password)
    {
        Name = name.Trim();
        Perfil = perfil;
        ValidateEmail(email);
        Email = email.Trim();
    }

    public void Update(string name, PerfilEnum perfil, string email, string password)
    {
        Name = name.Trim();
        Perfil = perfil;

        ValidateEmail(email);
        Email = email.Trim();

        SetPassword(password);
    }
}
