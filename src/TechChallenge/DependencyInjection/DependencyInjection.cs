using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            options.UseSqlServer(connectionString));

        // Registrar outros serviços de infra aqui, se necessário.
        // ex: services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}
