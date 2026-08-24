using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using TechChallenge.Domain.Interfaces;
using TechChallenge.Domain.Services;
using TechChallenge.Infrastructure.Authentication;
using TechChallenge.Infrastructure.Repository;

namespace TechChallenge.Configuration;

/// <summary>
/// Registra os serviços da camada de API (controllers, autenticação e Swagger).
/// </summary>
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
        services.AddScoped<IPromotionRepository, PromotionRepository>();
        services.AddScoped<IPromotionService,PromotionService>();
        services.AddScoped<IUserGameRepository, UserGameRepository>();
        services.AddScoped<IUserGameService, UserGameService>();

        return services;
    }

    /// <summary>
    /// Autenticação por token JWT, conforme exigido pelo desafio.
    /// As configurações vêm da seção "Jwt" do appsettings — as mesmas usadas pelo
    /// TokenService para assinar, de modo que assinatura e validação sempre batem.
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwt = configuration
            .GetSection(JwtSettings.SecaoConfiguracao)
            .Get<JwtSettings>()
            ?? throw new InvalidOperationException(
                "A seção 'Jwt' não foi encontrada na configuração.");

        if (jwt.SecretKey.Length < 32)
        {
            // O HMAC-SHA256 exige uma chave de 256 bits. Com menos que isso a
            // aplicação só falharia ao validar o primeiro token, em runtime.
            throw new InvalidOperationException(
                "A chave 'Jwt:SecretKey' precisa ter no mínimo 32 caracteres.");
        }

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwt.Issuer,
                ValidAudience = jwt.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwt.SecretKey)),

                // Sem isso o .NET aceita um token até 5 minutos após expirar.
                ClockSkew = TimeSpan.Zero
            };
        });

        return services;
    }

    /// <summary>
    /// Documentação da API no Swagger, com suporte a autenticação por token.
    /// </summary>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(swagger =>
        {
            swagger.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Tech Challenge",
                Version = "v1",
                Description = "API de cadastro de usuários e biblioteca de jogos da FIAP Cloud Games.",
                Contact = new OpenApiContact
                {
                    Name = "Equipe Desafio",
                    Email = "contato@desafio.com"
                }
            });

            // Campo para informar o token nas requisições feitas pelo Swagger.
            swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description =
                    "Informe o token no formato: Bearer {seu token}\n\n" +
                    "Atenção: chamar /auth/login NÃO troca o token daqui. " +
                    "Ao trocar de usuário, volte neste botão e cole o token novo — " +
                    "caso contrário as requisições continuarão sendo feitas como o usuário anterior.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Traz para o Swagger os comentários <summary> dos controllers.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath))
            {
                swagger.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }
}
