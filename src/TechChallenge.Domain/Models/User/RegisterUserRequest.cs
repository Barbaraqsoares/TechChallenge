namespace TechChallenge.Domain.Models.User;

public class RegisterUserRequest
{
    public string Name { get; set; }
    public  string Email { get; set; }
    public  string Login { get; set; }
    public string Password { get; set; }

}