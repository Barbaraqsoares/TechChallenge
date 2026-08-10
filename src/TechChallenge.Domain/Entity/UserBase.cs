using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TechChallenge.Domain.Entity;

public class UserBase
{
    public int Id { get; set; }
    public string Login { get; set; }
    public required string Password { get; set; }
    public PerfilEnum Perfil { get; set; }

    public DateTime CreatedAt { get; set; }

    public UserBase(string login, string password)
    {
        Login = login;
        Password = password;
    }

    public bool autenticar(string login, string password)
    {
        return this.Login.Equals(login) && this.Password.Equals(password);
    }

    public string getLogin()
    {
        return Login;
    }
}