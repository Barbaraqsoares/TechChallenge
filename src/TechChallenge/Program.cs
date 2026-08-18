using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using TechChallenge.Configuration;
using TechChallenge.HealthChecks;
using TechChallenge.Infrastructure;
using TechChallenge.Infrastructure.Database;
using TechChallenge.Middleware;

// Logger provisório, ativo apenas até o host subir com a configuração definitiva.
// Sem ele, uma falha antes do builder.Build() não seria registrada em lugar nenhum.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // ---------------------------------------------------------------------
    // Serviços da aplicação
    // ---------------------------------------------------------------------
    builder.Host.UseSerilogLogging();

    builder.Services.AddControllers();
    builder.Services.AddJwtAuthentication(builder.Configuration);
    builder.Services.AddSwaggerDocumentation();

    // Banco de dados e repositórios (camada de infraestrutura).
    builder.Services.AddInfrastructure(builder.Configuration);

    var app = builder.Build();

    // Deixa o banco atualizado e com o administrador inicial antes de a aplicação
    // começar a atender requisições.
    await app.Services.MigrateDatabaseAsync();
    await app.Services.SeedAsync();

    // ---------------------------------------------------------------------
    // Pipeline de requisições
    // ---------------------------------------------------------------------
    // A ordem importa: o log de requisições envolve tudo, e o tratamento de
    // exceções vem logo em seguida para capturar o que os demais lançarem.
    app.UseRequestLogging();
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.UseSwagger();
    app.UseSwaggerUI(swagger =>
    {
        swagger.SwaggerEndpoint("/swagger/v1/swagger.json", "Tech Challenge v1");
        swagger.RoutePrefix = string.Empty; // abre direto na raiz
    });

    // No container a aplicação serve apenas HTTP (o TLS fica a cargo do proxy à
    // frente dela). Manter o redirecionamento ali só geraria um aviso por requisição.
    if (app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // Responde 200 quando a aplicação está saudável e 503 quando não está.
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = HealthCheckResponseWriter.WriteAsync
    });

    app.Run();
}
// Erros de inicialização não passam pelo ExceptionHandlingMiddleware: ele só atua
// dentro de requisições HTTP. Exemplos: connection string inválida, porta em uso,
// migration que falha, serviço não registrado no DI.
// A cláusula "when" ignora o HostAbortedException, lançado pelas ferramentas do
// Entity Framework (dotnet ef migrations) ao encerrar o host propositalmente.
catch (Exception exception) when (exception is not HostAbortedException)
{
    Log.Fatal(exception, "A aplicação não pôde ser iniciada");

    // Encerra o processo com código de erro. É assim que o Docker e ferramentas de
    // orquestração percebem que o container falhou em vez de ter terminado normalmente.
    return 1;
}
finally
{
    // Grava o que ainda estiver em buffer antes do processo terminar.
    Log.CloseAndFlush();
}

return 0;
