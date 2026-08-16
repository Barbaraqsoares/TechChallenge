using Microsoft.EntityFrameworkCore;
using TechChallenge.Domain.Entity;

namespace TechChallenge.Infrastructure.Repository;

public class ApplicationDbContext : DbContext
{
    private readonly string _connectionString;

    public ApplicationDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Game> Games { get; set; }

    /* TO DOs
     * 1. Add DbSet properties for your entities here
     * Example:
     * public DbSet<YourEntity> YourEntities { get; set; }
     */

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        /* TO DOs
         * 1. Configure your entity mappings here
         * Criar classes de configuração, pasta configuration, para cada entidade e aplicar aqui
         * Example:
         * modelBuilder.ApplyConfigurationFromAssembly(typeOf(ApplicationDbContext).Assembly);
         */
    }
}
