using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TechChallenge.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _secret;

    public JwtMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _secret = config["Jwt:Secret"];
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_secret);

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);

                // Se quiser, pode adicionar claims ao contexto
                context.User = principal;
            }
            catch
            {
                // Token inválido → não autentica
                throw new Exception(string.Format("Token inválido"));
            }
        }

        await _next(context);
    }
}