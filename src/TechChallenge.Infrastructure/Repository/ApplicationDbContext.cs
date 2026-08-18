using Microsoft.EntityFrameworkCore;
using TechChallenge.Domain.Entity;

namespace TechChallenge.Infrastructure.Repository;

public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// As opções (provider e connection string) chegam prontas pela injeção de
    /// dependência, configuradas em DependencyInjection com AddDbContext.
    /// Este é também o construtor que as ferramentas do EF (dotnet ef) utilizam.
    /// </summary>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }

    public DbSet<Jogo> Jogos { get; set; }

    public DbSet<UsuarioJogo> UsuariosJogos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Carrega automaticamente toda classe IEntityTypeConfiguration<T> deste
        // projeto (pasta Repository/Configuration), sem precisar registrar uma a uma.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
