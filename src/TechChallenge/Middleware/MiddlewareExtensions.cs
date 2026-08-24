using Microsoft.AspNetCore.Builder;
using TechChallenge.Configuration;

namespace TechChallenge.Middleware;

public static class MiddlewareExtensions
{
    public static WebApplication UseExceptionHandling(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<JwtMiddleware>();
        return app;
    }

    public static WebApplication UseInfrastructurePipeline(this WebApplication app)
    {
        app.UseExceptionHandling();
        app.UseRequestLogging();

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
