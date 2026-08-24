using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using TechChallenge.Configuration;
using TechChallenge.Domain.Interfaces;
using TechChallenge.Infrastructure.Authentication;
using TechChallenge.Infrastructure.Repository;
using TechChallenge.Middleware;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilogLogging();

    var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

    // JWT settings via configuration (appsettings.json / environment)
    var jwtSection = builder.Configuration.GetSection("Jwt");
    builder.Services.Configure<JwtSettings>(jwtSection);
    var jwtSettings = jwtSection.Get<JwtSettings>() ?? new TechChallenge.Infrastructure.Authentication.JwtSettings();

    builder.Services.AddJwtAuthentication(builder.Configuration);

    builder.Services.AddScoped<ITokenService, TokenService>();

    // Add services to the container.

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Tech Challenge",
            Version = "v1",
            Description = "Tech Challenge",
            Contact = new OpenApiContact
            {
                Name = "Equipe Desafio",
                Email = "contato@desafio.com"
            }
        });

        // Configuração de segurança JWT
        c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Description = "Insira o token JWT no campo abaixo usando o esquema: Bearer {seu token}",
            Name = "Authorization",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
        });

        // Comentários XML (se habilitado no .csproj)
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
    });

    
    // Registrar serviços de infraestrutura (DbContext, repositórios, etc.)
    builder.Services.AddInfrastructure(builder.Configuration);

    var app = builder.Build();

    // Executa em qualquer ambiente
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await context.Database.MigrateAsync();

        await DbInitializer.InitializeAsync(context);
    }

    
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseRequestLogging();

    // Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TechChallange API v1");
        c.RoutePrefix = string.Empty; // abre direto na raiz
    });

    app.UseHttpsRedirection();

    // Ativar autenticação e autorização
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}

catch (Exception exception) when (exception is not HostAbortedException)
{
    Log.Fatal(exception, "A aplicação não pôde ser iniciada");


    return 1;
}
finally
{
    Log.CloseAndFlush();
}

return 0;