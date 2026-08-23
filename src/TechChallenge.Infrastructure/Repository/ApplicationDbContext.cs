using Microsoft.EntityFrameworkCore;
using TechChallenge.Domain.Entity;

namespace TechChallenge.Infrastructure.Repository;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    private readonly string _connectionString;

    public DbSet<User> Users { get; set; }
    public DbSet<Game> Games { get; set; }

    /* TO DOs
     * 1. Add DbSet properties for your entities here
     * Example:
     * public DbSet<YourEntity> YourEntities { get; set; }
     */
    // Removida propriedade duplicada 'User' para evitar confusão com 'Users'

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
