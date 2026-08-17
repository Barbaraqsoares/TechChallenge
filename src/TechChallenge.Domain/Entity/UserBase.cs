namespace TechChallenge.Domain.Entity;
using System.Net.Mail;

public class UserBase
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string Password { get; protected set; }
    public PerfilEnum Perfil { get; set; }

    public DateTime CreatedAt { get; protected set; }

    public UserBase(string login, string password)
    {
        Login = login;
        SetPassword(password);
        CreatedAt = DateTime.Now;
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

    public bool autenticar(string login, string password)
    {
        return this.Login.Equals(login) && this.Password.Equals(password);
    }

    public string getLogin()
    {
        return Login;
    }


}