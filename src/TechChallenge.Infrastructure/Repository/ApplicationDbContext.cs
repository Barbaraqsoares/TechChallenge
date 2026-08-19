using Microsoft.EntityFrameworkCore;
using TechChallenge.Domain.Entity;

namespace TechChallenge.Infrastructure.Repository;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options ) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Promotion> Promotions { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Promotion>()
            .HasMany(promotion => promotion.Games)
            .WithMany(game => game.Promotions)
            .UsingEntity(join => join.ToTable("GamePromotions"));
    }
}
