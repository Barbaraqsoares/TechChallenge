using Microsoft.EntityFrameworkCore;
using TechChallenge.Domain.Entity;
using TechChallenge.Infrastructure.Repository;

public static class DbInitializer
{
    public static async Task InitializeAsync(
        ApplicationDbContext context)
    {
        var adminExists = await context.Users
            .AnyAsync(x => x.Login == "admin");

        if (adminExists)
        {
            return;
        }

        var admin = new User(
            "Administrator",
            "admin@fiap.com",
            "admin",
            "Admin@123",
            PerfilEnum.Admin
        );

        context.Users.Add(admin);

        await context.SaveChangesAsync();
    }
}