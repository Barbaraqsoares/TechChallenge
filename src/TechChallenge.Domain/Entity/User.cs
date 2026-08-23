using System.Net.Mail;
using System.Net.NetworkInformation;

namespace TechChallenge.Domain.Entity;

public class User : EntityBAse
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public PerfilEnum Perfil { get; set; }

    public User(string name, string email, string login, string passaword, PerfilEnum perfil)
    {
        Name = name.Trim();        
        ValidateEmail(email);
        Email = email.Trim();
        Login = login;     
        ValidatePassword(passaword);
        Password = passaword;
        Perfil = perfil;
    }



    public void Update(string name, string email, string password, PerfilEnum perfil)
    {
        Name = name.Trim();
        Perfil = perfil;

        ValidateEmail(email);
        Email = email.Trim();

        SetPassword(password);
    }

    protected void SetPassword(string password)
    {
        ValidatePassword(password);
        Password = password;
    }
    private static void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("A senha é obrigatória.");
        }

        if (password.Length < 8)
        {
            throw new ArgumentException(
                "A senha deve possuir no mínimo 8 caracteres."
            );
        }

        if (!password.Any(char.IsLetter))
        {
            throw new ArgumentException(
                "A senha deve possuir pelo menos uma letra."
            );
        }

        if (!password.Any(char.IsDigit))
        {
            throw new ArgumentException(
                "A senha deve possuir pelo menos um número."
            );
        }

        if (!password.Any(c => !char.IsLetterOrDigit(c)))
        {
            throw new ArgumentException(
                "A senha deve possuir pelo menos um caractere especial."
            );
        }
    }

    protected static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("O e-mail é obrigatório.");
        }

        try
        {
            var mailAddress = new MailAddress(email);

            if (mailAddress.Address != email)
            {
                throw new ArgumentException("E-mail inválido.");
            }
        }
        catch (FormatException)
        {
            throw new ArgumentException("E-mail inválido.");
        }
    }
}
