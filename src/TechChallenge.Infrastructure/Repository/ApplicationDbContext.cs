using Microsoft.EntityFrameworkCore;

namespace TechChallenge.Infrastructure.Repository;

public class ApplicationDbContext : DbContext
{
    private readonly string _connectionString;

    public ApplicationDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

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
        /* TO DOs
         * 1. Configure your entity mappings here
         * Criar classes de configuração, pasta configuration, para cada entidade e aplicar aqui
         * Example:
         * modelBuilder.ApplyConfigurationFromAssembly(typeOf(ApplicationDbContext).Assembly);
         */
    }
}
