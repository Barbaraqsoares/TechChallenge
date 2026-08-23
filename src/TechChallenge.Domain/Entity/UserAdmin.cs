namespace TechChallenge.Domain.Entity;

public class UserAdmin : UserBase
{
    public string Name { get; set; }
    public string Email { get; set; }

    public UserAdmin(string name, PerfilEnum perfil, string email, string password, string login)
        : base(login, password)
    {
        Name = name.Trim();
        Perfil = perfil;
        ValidateEmail(email);
        Email = email.Trim();

        
        Login = login;
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