using Microsoft.EntityFrameworkCore;
using TechChallenge.Domain.Interfaces;
using TechChallenge.Domain.Services;
using TechChallenge.Infrastructure.Repository;

namespace TechChallenge.DependencyInjection;

public static class DependencyInjection
{
    /// <summary>
    /// Registra serviços de infraestrutura (DbContext, repositories, etc.).
    /// Mantém Program.cs enxuto delegando a configuração aqui.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Tenta obter connection string nomeada "SqlConnection" ou fallback para "DefaultConnection"
        var connectionString = configuration.GetConnectionString("SqlConnection")
            ?? configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(
                connectionString,
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure();
                }
            );
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();

        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IGameService, GameService>();

        // Registrar outros serviços de infra aqui, se necessário.
        // ex: services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}
