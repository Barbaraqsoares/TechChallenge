using Microsoft.EntityFrameworkCore;
using TechChallenge.Domain.Entity;

namespace TechChallenge.Infrastructure.Repository;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{ 
    public DbSet<User> Users { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Promotion> Promotions { get; set; }
    public DbSet<UserGame> UserGames { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}