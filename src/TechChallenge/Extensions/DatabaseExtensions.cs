using Microsoft.EntityFrameworkCore;
using TechChallenge.Infrastructure.Repository.Configuration;
using TechChallenge.Infrastructure.Repository;

namespace TechChallenge.Extensions;

public static class DatabaseExtensions
{
    public static async Task MigrateAndSeedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await context.Database.MigrateAsync();
        await DbInitializer.InitializeAsync(context);
    }
}