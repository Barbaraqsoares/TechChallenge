using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TechChallenge.Infrastructure.Repository;

namespace TechChallenge.Infrastructure.Database;

/// <summary>
/// Aplica as migrations pendentes durante a inicialização da aplicação, para que
/// o banco fique pronto sem ninguém precisar rodar "dotnet ef database update".
/// É o que permite subir tudo com um único "docker-compose up".
/// </summary>
public static class DatabaseMigrationExtensions
{
    /// <summary>
    /// Verifica se há migrations pendentes e as aplica, registrando o resultado no log.
    /// Recebe IServiceProvider (e não WebApplication) para que esta camada não
    /// precise depender do ASP.NET Core.
    /// </summary>
    public static async Task MigrateDatabaseAsync(this IServiceProvider services)
    {
        // O DbContext é registrado com tempo de vida Scoped, então precisa de um
        // escopo próprio: aqui ainda não existe requisição HTTP para fornecer um.
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger(typeof(DatabaseMigrationExtensions));

        var pendingMigrations = (await context.Database.GetPendingMigrationsAsync()).ToList();

        if (pendingMigrations.Count == 0)
        {
            logger.LogInformation("Banco de dados já está atualizado. Nenhuma migration pendente.");
            return;
        }

        logger.LogInformation(
            "Aplicando {PendingMigrationCount} migration(s) pendente(s): {PendingMigrations}",
            pendingMigrations.Count,
            pendingMigrations);

        await context.Database.MigrateAsync();

        logger.LogInformation("Migrations aplicadas com sucesso.");
    }
}
