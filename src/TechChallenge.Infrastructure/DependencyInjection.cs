using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechChallenge.Domain.Interfaces;
using TechChallenge.Infrastructure.Authentication;
using TechChallenge.Infrastructure.Repository;

namespace TechChallenge.Infrastructure;

/// <summary>
/// Registra os serviços da camada de infraestrutura (banco de dados, repositórios
/// e geração de token).
///
/// Manter este registro aqui evita que a camada de API precise conhecer detalhes
/// de persistência, como o provider do Entity Framework ou a connection string.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("SqlConnection"),
                sqlServer => sqlServer.EnableRetryOnFailure(
                    // O SQL Server em container leva alguns segundos para aceitar
                    // conexões. Sem estas tentativas, a aplicação subiria antes do
                    // banco e falharia.
                    maxRetryCount: 10,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null)));

        // Scoped: uma instância por requisição HTTP, o mesmo tempo de vida do
        // DbContext que os repositórios utilizam.
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IJogoRepository, JogoRepository>();

        // Configurações do JWT lidas da seção "Jwt" do appsettings.
        services.Configure<JwtSettings>(
            configuration.GetSection(JwtSettings.SecaoConfiguracao));

        services.AddScoped<ITokenService, TokenService>();

        // Verificação do banco exposta no endpoint /health, sob o nome "database".
        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>("database");

        return services;
    }
}
