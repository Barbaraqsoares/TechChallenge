namespace TechChallenge.DependencyInjection;

public class JwtSettings
{
    public string Secret { get; set; } = "chave-secreta-super-segura";
    public string Issuer { get; set; } = "suaempresa.com";
    public string Audience { get; set; } = "suaempresa.com";
    public int ExpireHours { get; set; } = 1;

}