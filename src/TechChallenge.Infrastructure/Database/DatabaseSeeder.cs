using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TechChallenge.Domain.Entity;
using TechChallenge.Infrastructure.Repository;

namespace TechChallenge.Infrastructure.Database;

/// <summary>
/// Cria o administrador inicial da plataforma.
///
/// Sem ele haveria um impasse: cadastrar jogos exige perfil de administrador, e
/// todo usuário criado pelo endpoint público nasce como Cliente — ninguém
/// conseguiria promover ninguém.
///
/// O seed roda em código (e não com HasData) porque o hash da senha usa um salt
/// aleatório: um valor fixo na migration mudaria a cada geração.
/// </summary>
public static class DatabaseSeeder
{
    private const string EmailDoAdministrador = "admin@fiapcloudgames.com";
    private const string SenhaDoAdministrador = "Admin@123";

    public static async Task SeedAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger(typeof(DatabaseSeeder));

        if (await context.Usuarios.AnyAsync())
        {
            return;
        }

        var administrador = Usuario.Criar(
            nome: "Administrador",
            email: EmailDoAdministrador,
            senha: SenhaDoAdministrador,
            perfil: PerfilEnum.Admin);

        context.Usuarios.Add(administrador);
        await context.SaveChangesAsync();

        logger.LogWarning(
            "Administrador inicial criado com o e-mail {Email} e senha padrão. " +
            "Altere a senha antes de usar em produção.",
            EmailDoAdministrador);
    }
}
