using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechChallenge.Infrastructure.Repository.Configuration;
using TechChallenge.Infrastructure.Repository;

namespace TechChallenge.Extensions;

public static class DatabaseExtensions
{
    public static async Task MigrateAndSeedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var loggerFactory = scope.ServiceProvider.GetService<ILoggerFactory>();
        var logger = loggerFactory?.CreateLogger("DatabaseExtensions");

        try
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await context.Database.MigrateAsync();
            await DbInitializer.InitializeAsync(context);
        }
        catch (Exception ex)
        {
            // Loga o erro e permite que a aplicação continue inicializando
            logger?.LogError(ex, "Falha ao aplicar migrations/seed no banco. Verifique a connection string e o servidor de banco.");
        }
    }
}
