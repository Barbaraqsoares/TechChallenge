using Microsoft.AspNetCore.Builder;
using TechChallenge.Configuration;

namespace TechChallenge.Middleware;

public static class MiddlewareExtensions
{
    /// <summary>
    /// Registra o tratamento centralizado de exceções.
    ///
    /// A validação do token NÃO entra aqui: quem faz isso é o UseAuthentication(),
    /// configurado em AddJwtAuthentication(). Ele já confere assinatura, emissor,
    /// audiência e expiração, e devolve 401 sozinho quando o token não presta.
    /// </summary>
    public static WebApplication UseExceptionHandling(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        return app;
    }

    /// <summary>
    /// Monta o pipeline de requisições. A ordem aqui é o próprio comportamento da
    /// aplicação: cada middleware envolve todos os que vêm depois dele.
    /// </summary>
    public static WebApplication UseInfrastructurePipeline(this WebApplication app)
    {
        // O log de requisições vem primeiro para envolver tudo, inclusive o
        // tratamento de exceções. Invertido, ele veria a exceção antes de virar
        // 404/400/409 e registraria toda falha de negócio como erro 500 —
        // contradizendo o status que o cliente de fato recebeu.
        app.UseRequestLogging();
        app.UseExceptionHandling();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "TechChallenge API v1");
            c.RoutePrefix = string.Empty;
        });

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}
